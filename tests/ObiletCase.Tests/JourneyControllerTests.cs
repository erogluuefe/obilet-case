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

        Assert.Equal(9, model.Journeys[0].Departure.Hour);
        Assert.Equal(11, model.Journeys[1].Departure.Hour);
        Assert.Equal(14, model.Journeys[2].Departure.Hour);
    }

    [Fact]
    public async Task Index_PutsWhatTheCardShowsIntoTheCard()
    {
        var apiClient = new FakeObiletApiClient();
        apiClient.Journeys = new List<BusJourney>
        {
            new BusJourney
            {
                PartnerName = "Metro Turizm",
                Journey = new JourneyDetails
                {
                    Origin = "Esenler Otogarı",
                    Destination = "Ankara (Aşti) Otogarı",
                    Departure = new DateTime(2030, 5, 20, 9, 30, 0),
                    Arrival = new DateTime(2030, 5, 20, 15, 30, 0),
                    InternetPrice = 545m,
                    Currency = "TRY"
                }
            }
        };
        var controller = new JourneyController(new FakeObiletSessionService(), apiClient);

        var model = await GetIndexModel(controller);

        var card = Assert.Single(model.Journeys);
        Assert.Equal("Metro Turizm", card.PartnerName);
        Assert.Equal("Esenler Otogarı", card.OriginStop);
        Assert.Equal("Ankara (Aşti) Otogarı", card.DestinationStop);
        Assert.Equal(new DateTime(2030, 5, 20, 9, 30, 0), card.Departure);
        Assert.Equal(new DateTime(2030, 5, 20, 15, 30, 0), card.Arrival);
        Assert.Equal(545m, card.Price);
    }

    [Fact]
    public async Task Index_ShowsTurkishLiraAsTL()
    {
        var apiClient = new FakeObiletApiClient();
        apiClient.Journeys = new List<BusJourney> { CreateJourney(departureHour: 9, currency: "TRY") };
        var controller = new JourneyController(new FakeObiletSessionService(), apiClient);

        var model = await GetIndexModel(controller);

        Assert.Equal("TL", model.Journeys[0].CurrencyLabel);
    }

    [Fact]
    public async Task Index_ShowsOtherCurrenciesWithTheirCode()
    {
        var apiClient = new FakeObiletApiClient();
        apiClient.Journeys = new List<BusJourney> { CreateJourney(departureHour: 9, currency: "UAH") };
        var controller = new JourneyController(new FakeObiletSessionService(), apiClient);

        var model = await GetIndexModel(controller);

        Assert.Equal("UAH", model.Journeys[0].CurrencyLabel);
    }

    [Fact]
    public async Task Index_MarksJourneysThatLeaveAfterMidnight()
    {
        var apiClient = new FakeObiletApiClient();
        var lateEvening = CreateJourney(departureHour: 23);
        var afterMidnight = new BusJourney
        {
            Journey = new JourneyDetails { Departure = new DateTime(2030, 5, 21, 0, 5, 0) }
        };
        apiClient.Journeys = new List<BusJourney> { afterMidnight, lateEvening };
        var controller = new JourneyController(new FakeObiletSessionService(), apiClient);

        var model = await GetIndexModel(controller);

        Assert.False(model.Journeys[0].DepartsNextDay);
        Assert.True(model.Journeys[1].DepartsNextDay);
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

    // A journey on the searched day (20 May 2030), leaving at the given hour.
    private static BusJourney CreateJourney(int departureHour, string currency = "TRY")
    {
        return new BusJourney
        {
            OriginLocation = "İstanbul Avrupa",
            DestinationLocation = "Ankara",
            Journey = new JourneyDetails
            {
                Departure = new DateTime(2030, 5, 20, departureHour, 0, 0),
                Currency = currency
            }
        };
    }

    private static async Task<JourneyListViewModel> GetIndexModel(JourneyController controller)
    {
        var result = await controller.Index(CreateSearch(), CancellationToken.None);
        var viewResult = Assert.IsType<ViewResult>(result);

        return Assert.IsType<JourneyListViewModel>(viewResult.Model);
    }
}
