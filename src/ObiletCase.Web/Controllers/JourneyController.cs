using Microsoft.AspNetCore.Mvc;
using ObiletCase.Web.Models;
using ObiletCase.Web.Services.Obilet;
using ObiletCase.Web.Services.Obilet.Models;

namespace ObiletCase.Web.Controllers;

/// <summary>Serves the page that lists the bus journeys of a search.</summary>
public class JourneyController : Controller
{
    private readonly IObiletSessionService _sessionService;
    private readonly IObiletApiClient _apiClient;

    public JourneyController(IObiletSessionService sessionService, IObiletApiClient apiClient)
    {
        _sessionService = sessionService;
        _apiClient = apiClient;
    }

    /// <summary>Lists the journeys of the search in the address, ordered by departure time.</summary>
    [HttpGet]
    public async Task<IActionResult> Index(JourneySearchViewModel search, CancellationToken cancellationToken)
    {
        // The address can be typed or changed by hand, so the search is checked with the same rules as the form.
        if (!ModelState.IsValid)
        {
            return RedirectToAction("Index", "Home");
        }

        var session = await _sessionService.GetSessionAsync(cancellationToken);
        var journeys = await _apiClient.GetBusJourneysAsync(session, search.OriginId, search.DestinationId, search.DepartureDate, cancellationToken);

        var model = new JourneyListViewModel
        {
            OriginId = search.OriginId,
            OriginName = search.OriginName,
            DestinationId = search.DestinationId,
            DestinationName = search.DestinationName,
            DepartureDate = search.DepartureDate
        };

        // The names in the address can be changed by hand; when there are journeys, obilet's own names are shown.
        if (journeys.Count > 0)
        {
            model.OriginName = journeys[0].OriginLocation;
            model.DestinationName = journeys[0].DestinationLocation;
        }

        // obilet does not send the journeys in time order.
        var journeysByDeparture = journeys.OrderBy(journey => journey.Journey.Departure);

        foreach (var journey in journeysByDeparture)
        {
            model.Journeys.Add(CreateCard(journey, search.DepartureDate));
        }

        return View(model);
    }

    // Turns a journey from obilet into what the page shows, so the view does not have to decide anything.
    private static JourneyCardViewModel CreateCard(BusJourney journey, DateOnly searchedDate)
    {
        var card = new JourneyCardViewModel
        {
            PartnerName = journey.PartnerName,
            OriginStop = journey.Journey.Origin,
            DestinationStop = journey.Journey.Destination,
            Departure = journey.Journey.Departure,
            Arrival = journey.Journey.Arrival,
            Price = journey.Journey.InternetPrice,
            CurrencyLabel = journey.Journey.Currency
        };

        // obilet also returns journeys that leave shortly after midnight; the page marks them.
        card.DepartsNextDay = DateOnly.FromDateTime(journey.Journey.Departure) != searchedDate;

        // obilet sends the currency as a code; Turkish lira is shown the way it is written in Turkey.
        if (journey.Journey.Currency == "TRY")
        {
            card.CurrencyLabel = "TL";
        }

        return card;
    }
}
