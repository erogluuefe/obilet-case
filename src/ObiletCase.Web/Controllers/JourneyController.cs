using Microsoft.AspNetCore.Mvc;
using ObiletCase.Web.Models;
using ObiletCase.Web.Services.Obilet;

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
            OriginName = search.OriginName,
            DestinationName = search.DestinationName,
            DepartureDate = search.DepartureDate,

            // obilet does not send the journeys in time order.
            Journeys = journeys.OrderBy(journey => journey.Journey.Departure).ToList()
        };

        // The names in the address can be changed by hand; when there are journeys, obilet's own names are shown.
        if (model.Journeys.Count > 0)
        {
            model.OriginName = model.Journeys[0].OriginLocation;
            model.DestinationName = model.Journeys[0].DestinationLocation;
        }

        return View(model);
    }
}
