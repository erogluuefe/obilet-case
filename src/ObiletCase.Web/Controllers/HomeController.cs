using System.Diagnostics;
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

    /// <summary>Shows the journey search form.</summary>
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var session = await _sessionService.GetSessionAsync(cancellationToken);
        var locations = await _apiClient.GetBusLocationsAsync(session, searchText: null, cancellationToken);

        var model = new JourneySearchViewModel();
        model.Locations = locations;
        model.Today = DateOnly.FromDateTime(DateTime.Today);
        model.Tomorrow = model.Today.AddDays(1);
        model.DepartureDate = model.Tomorrow;

        // The default origin and destination follow the order obilet returns the locations in.
        if (locations.Count > 0)
        {
            model.OriginId = locations[0].Id;
        }

        if (locations.Count > 1)
        {
            model.DestinationId = locations[1].Id;
        }

        return View(model);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
