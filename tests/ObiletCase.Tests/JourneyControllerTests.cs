using Microsoft.AspNetCore.Mvc;
using ObiletCase.Web.Controllers;
using ObiletCase.Web.Models;
using ObiletCase.Web.Services.Obilet.Models;

namespace ObiletCase.Tests;

public class JourneyControllerTests
{
    [Fact]
    public async Task Index_AsksObiletForTheSearchWithTheVisitorSession()
    {
        var sessionService = new FakeObiletSessionService();
        var apiClient = new FakeObiletApiClient();
        var controller = new JourneyController(sessionService, apiClient);

        await controller.Index(CreateSearch(), CancellationToken.None);

        Assert.Same(sessionService.Session, apiClient.LastJourneysSession);
        Assert.Equal(349, apiClient.LastOriginId);
        Assert.Equal(356, apiClient.LastDestinationId);
        Assert.Equal(new DateOnly(2030, 5, 20), apiClient.LastDepartureDate);
    }

    [Fact]
    public async Task Index_ShowsTheJourneysOrderedByDepartureTime()
    {
        var apiClient = new FakeObiletApiClient();
        apiClient.Journeys = new List<BusJourney>
        {
            CreateJourney(departureHour: 14),
            CreateJourney(departureHour: 9),
            CreateJourney(departureHour: 11)
        };
        var controller = new JourneyController(new FakeObiletSessionService(), apiClient);

        var model = await GetIndexModel(controller);

        Assert.Equal(9, model.Journeys[0].Journey.Departure.Hour);
        Assert.Equal(11, model.Journeys[1].Journey.Departure.Hour);
        Assert.Equal(14, model.Journeys[2].Journey.Departure.Hour);
    }

    [Fact]
    public async Task Index_PassesTheSearchToThePageSoItCanBeRemembered()
    {
        var controller = new JourneyController(new FakeObiletSessionService(), new FakeObiletApiClient());

        var model = await GetIndexModel(controller);

        Assert.Equal(349, model.OriginId);
        Assert.Equal(356, model.DestinationId);
        Assert.Equal(new DateOnly(2030, 5, 20), model.DepartureDate);
    }

    [Fact]
    public async Task Index_ShowsTheNamesObiletSentWithTheJourneys()
    {
        var apiClient = new FakeObiletApiClient();
        apiClient.Journeys = new List<BusJourney> { CreateJourney(departureHour: 9) };
        var controller = new JourneyController(new FakeObiletSessionService(), apiClient);

        var model = await GetIndexModel(controller);

        Assert.Equal("İstanbul Avrupa", model.OriginName);
        Assert.Equal("Ankara", model.DestinationName);
        Assert.Equal(new DateOnly(2030, 5, 20), model.DepartureDate);
    }

    [Fact]
    public async Task Index_WithoutJourneys_ShowsTheNamesOfTheSearch()
    {
        var controller = new JourneyController(new FakeObiletSessionService(), new FakeObiletApiClient());

        var model = await GetIndexModel(controller);

        Assert.Empty(model.Journeys);
        Assert.Equal("name from the search", model.OriginName);
        Assert.Equal("other name from the search", model.DestinationName);
    }

    [Fact]
    public async Task Index_WithInvalidSearch_GoesBackToTheSearchPageWithoutAskingObilet()
    {
        var apiClient = new FakeObiletApiClient();
        var controller = new JourneyController(new FakeObiletSessionService(), apiClient);

        // In a unit test MVC does not run the validation rules, so the error is added by hand.
        controller.ModelState.AddModelError(nameof(JourneySearchViewModel.DepartureDate), "Yolculuk tarihi bugünden önce olamaz.");

        var result = await controller.Index(CreateSearch(), CancellationToken.None);

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Home", redirect.ControllerName);
        Assert.Equal("Index", redirect.ActionName);
        Assert.Null(apiClient.LastJourneysSession);
    }

    private static JourneySearchViewModel CreateSearch()
    {
        return new JourneySearchViewModel
        {
            OriginId = 349,
            OriginName = "name from the search",
            DestinationId = 356,
            DestinationName = "other name from the search",
            DepartureDate = new DateOnly(2030, 5, 20)
        };
    }

    private static BusJourney CreateJourney(int departureHour)
    {
        return new BusJourney
        {
            OriginLocation = "İstanbul Avrupa",
            DestinationLocation = "Ankara",
            Journey = new JourneyDetails { Departure = new DateTime(2030, 5, 20, departureHour, 0, 0) }
        };
    }

    private static async Task<JourneyListViewModel> GetIndexModel(JourneyController controller)
    {
        var result = await controller.Index(CreateSearch(), CancellationToken.None);
        var viewResult = Assert.IsType<ViewResult>(result);

        return Assert.IsType<JourneyListViewModel>(viewResult.Model);
    }
}
