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
        Assert.Equal(350, model.DestinationId);
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
    public async Task Index_AsksObiletWithTheVisitorSession()
    {
        var sessionService = new FakeObiletSessionService();
        var apiClient = CreateApiClientWithThreeLocations();
        var controller = new HomeController(sessionService, apiClient);

        await controller.Index(CancellationToken.None);

        Assert.Same(sessionService.Session, apiClient.LastLocationsSession);
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
