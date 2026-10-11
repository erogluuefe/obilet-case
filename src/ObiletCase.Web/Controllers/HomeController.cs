using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ObiletCase.Web.Models;
using ObiletCase.Web.Services.Obilet;

namespace ObiletCase.Web.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IObiletSessionService _sessionService;

    public HomeController(ILogger<HomeController> logger, IObiletSessionService sessionService)
    {
        _logger = logger;
        _sessionService = sessionService;
    }

    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        // Creates the visitor's obilet session on their first visit.
        await _sessionService.GetSessionAsync(cancellationToken);

        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
