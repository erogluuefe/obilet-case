using Microsoft.AspNetCore.Mvc;
using ObiletCase.Web.Controllers;
using ObiletCase.Web.Models;
using ObiletCase.Web.Services.Obilet.Models;

namespace ObiletCase.Tests;

public class HomeControllerTests
{
    [Fact]
    public async Task Index_ShowsTheLocationsFromObilet()
    {
        var apiClient = CreateApiClientWithThreeLocations();
        var controller = new HomeController(new FakeObiletSessionService(), apiClient);

        var model = await GetIndexModel(controller);

        Assert.Equal(3, model.Locations.Count);
        Assert.Null(apiClient.LastSearchText);
    }

    [Fact]
    public async Task Index_SelectsTheFirstTwoLocationsAsOriginAndDestination()
    {
        var apiClient = CreateApiClientWithThreeLocations();
        var controller = new HomeController(new FakeObiletSessionService(), apiClient);

        var model = await GetIndexModel(controller);

        Assert.Equal(349, model.OriginId);
        Assert.Equal("İstanbul Avrupa", model.OriginName);
        Assert.Equal(350, model.DestinationId);
        Assert.Equal("İstanbul Anadolu", model.DestinationName);
    }

    [Fact]
    public async Task Index_SelectsTomorrowAsDepartureDate()
    {
        var apiClient = CreateApiClientWithThreeLocations();
        var controller = new HomeController(new FakeObiletSessionService(), apiClient);

        var model = await GetIndexModel(controller);

        var tomorrow = DateOnly.FromDateTime(DateTime.Today.AddDays(1));
        Assert.Equal(tomorrow, model.DepartureDate);
    }

    [Fact]
    public async Task Index_ProvidesTodayAndTomorrowForTheDateButtons()
    {
        var apiClient = CreateApiClientWithThreeLocations();
        var controller = new HomeController(new FakeObiletSessionService(), apiClient);

        var model = await GetIndexModel(controller);

        var today = DateOnly.FromDateTime(DateTime.Today);
        Assert.Equal(today, model.Today);
        Assert.Equal(today.AddDays(1), model.Tomorrow);
    }

    [Fact]
    public async Task Index_AsksObiletWithTheVisitorSession()
    {
        var sessionService = new FakeObiletSessionService();
        var apiClient = CreateApiClientWithThreeLocations();
        var controller = new HomeController(sessionService, apiClient);

        await controller.Index(CancellationToken.None);

        Assert.Same(sessionService.Session, apiClient.LastLocationsSession);
    }

    [Fact]
    public async Task IndexPost_WithInvalidSearch_ShowsTheFormAgainWithTheVisitorsChoices()
    {
        var apiClient = CreateApiClientWithThreeLocations();
        var controller = new HomeController(new FakeObiletSessionService(), apiClient);
        var search = CreateSearch(originId: 349, destinationId: 349);

        // In a unit test MVC does not run the validation rules, so the error is added by hand.
        controller.ModelState.AddModelError(nameof(JourneySearchViewModel.DestinationId), "Kalkış ve varış noktası aynı olamaz.");

        var result = await controller.Index(search, CancellationToken.None);

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<JourneySearchViewModel>(viewResult.Model);
        Assert.Equal(3, model.Locations.Count);
        Assert.Equal(349, model.OriginId);
        Assert.Equal(349, model.DestinationId);
        Assert.Equal(DateOnly.FromDateTime(DateTime.Today), model.Today);
    }

    [Fact]
    public async Task IndexPost_WithValidSearch_RedirectsToTheJourneyPage()
    {
        var apiClient = CreateApiClientWithThreeLocations();
        var controller = new HomeController(new FakeObiletSessionService(), apiClient);
        var search = CreateSearch(originId: 349, destinationId: 356);

        var result = await controller.Index(search, CancellationToken.None);

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Journey", redirect.ControllerName);
        Assert.Equal("Index", redirect.ActionName);
        Assert.Equal(349, redirect.RouteValues!["OriginId"]);
        Assert.Equal("origin name", redirect.RouteValues["OriginName"]);
        Assert.Equal(356, redirect.RouteValues["DestinationId"]);
        Assert.Equal("destination name", redirect.RouteValues["DestinationName"]);
        Assert.Equal("2030-05-20", redirect.RouteValues["DepartureDate"]);
    }

    [Fact]
    public async Task SearchLocations_AsksObiletWithTheTypedTextAndTheVisitorSession()
    {
        var sessionService = new FakeObiletSessionService();
        var apiClient = CreateApiClientWithThreeLocations();
        var controller = new HomeController(sessionService, apiClient);

        await controller.SearchLocations("ank", CancellationToken.None);

        Assert.Equal("ank", apiClient.LastSearchText);
        Assert.Same(sessionService.Session, apiClient.LastLocationsSession);
    }

    [Fact]
    public async Task SearchLocations_ReturnsTheLocationsAsJson()
    {
        var apiClient = CreateApiClientWithThreeLocations();
        var controller = new HomeController(new FakeObiletSessionService(), apiClient);

        var result = await controller.SearchLocations("ank", CancellationToken.None);

        var jsonResult = Assert.IsType<JsonResult>(result);
        var locations = Assert.IsType<List<BusLocation>>(jsonResult.Value);
        Assert.Equal(3, locations.Count);
    }

    private static JourneySearchViewModel CreateSearch(int originId, int destinationId)
    {
        return new JourneySearchViewModel
        {
            OriginId = originId,
            OriginName = "origin name",
            DestinationId = destinationId,
            DestinationName = "destination name",
            DepartureDate = new DateOnly(2030, 5, 20)
        };
    }

    private static FakeObiletApiClient CreateApiClientWithThreeLocations()
    {
        var apiClient = new FakeObiletApiClient();

        apiClient.Locations = new List<BusLocation>
        {
            new BusLocation { Id = 349, Name = "İstanbul Avrupa" },
            new BusLocation { Id = 350, Name = "İstanbul Anadolu" },
            new BusLocation { Id = 356, Name = "Ankara" }
        };

        return apiClient;
    }

    private static async Task<JourneySearchViewModel> GetIndexModel(HomeController controller)
    {
        var result = await controller.Index(CancellationToken.None);
        var viewResult = Assert.IsType<ViewResult>(result);

        return Assert.IsType<JourneySearchViewModel>(viewResult.Model);
    }
}
