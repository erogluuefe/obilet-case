using Microsoft.AspNetCore.Mvc;
using ObiletCase.Web.Models;
using ObiletCase.Web.Services.Obilet;

namespace ObiletCase.Web.Controllers;

/// <summary>Serves the journey search page.</summary>
public class HomeController : Controller
{
    private readonly IObiletSessionService _sessionService;
    private readonly IObiletApiClient _apiClient;

    public HomeController(IObiletSessionService sessionService, IObiletApiClient apiClient)
    {
        _sessionService = sessionService;
        _apiClient = apiClient;
    }

    /// <summary>Shows the journey search form with its default values.</summary>
    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var model = new JourneySearchViewModel();
        await FillFormOptionsAsync(model, cancellationToken);

        model.DepartureDate = model.Tomorrow;

        // The default origin and destination follow the order obilet returns the locations in.
        if (model.Locations.Count > 0)
        {
            model.OriginId = model.Locations[0].Id;
            model.OriginName = model.Locations[0].Name;
        }

        if (model.Locations.Count > 1)
        {
            model.DestinationId = model.Locations[1].Id;
            model.DestinationName = model.Locations[1].Name;
        }

        return View(model);
    }

    /// <summary>Checks the submitted search and sends the visitor to the journey page when it is valid.</summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(JourneySearchViewModel model, CancellationToken cancellationToken)
    {
        // MVC has already run the validation rules of the model and stored the result in ModelState.
        if (!ModelState.IsValid)
        {
            await FillFormOptionsAsync(model, cancellationToken);

            return View(model);
        }

        // The names travel with the ids, so the journey page can show them even when no journey is found.
        var journeySearch = new
        {
            model.OriginId,
            model.OriginName,
            model.DestinationId,
            model.DestinationName,
            DepartureDate = model.DepartureDate.ToString("O")
        };

        return RedirectToAction("Index", "Journey", journeySearch);
    }

    /// <summary>Returns the locations that match the typed text as JSON; called by the search boxes of the form.</summary>
    [HttpGet]
    public async Task<IActionResult> SearchLocations(string? text, CancellationToken cancellationToken)
    {
        var session = await _sessionService.GetSessionAsync(cancellationToken);
        var locations = await _apiClient.GetBusLocationsAsync(session, text, cancellationToken);

        return Json(locations);
    }

    // Loads what the form shows besides the visitor's own choices: the location list and the shortcut dates.
    private async Task FillFormOptionsAsync(JourneySearchViewModel model, CancellationToken cancellationToken)
    {
        var session = await _sessionService.GetSessionAsync(cancellationToken);

        model.Locations = await _apiClient.GetBusLocationsAsync(session, searchText: null, cancellationToken);
        model.Today = DateOnly.FromDateTime(DateTime.Today);
        model.Tomorrow = model.Today.AddDays(1);
    }

    /// <summary>Shows the error page; unexpected errors are sent here outside the development environment.</summary>
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View();
    }
}
