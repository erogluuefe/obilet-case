using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ObiletCase.Web.Services.Obilet;

namespace ObiletCase.Web.Filters;

/// <summary>Shows the error page instead of a crash when obilet fails: an error answer, no connection or no answer in time.</summary>
public class ObiletExceptionFilter : IExceptionFilter
{
    private readonly ILogger<ObiletExceptionFilter> _logger;

    public ObiletExceptionFilter(ILogger<ObiletExceptionFilter> logger)
    {
        _logger = logger;
    }

    public void OnException(ExceptionContext context)
    {
        // Anything else is not an obilet failure and keeps its normal handling.
        if (!IsObiletFailure(context.Exception))
        {
            return;
        }

        _logger.LogError(context.Exception, "A request to the obilet API failed.");

        context.Result = new ViewResult
        {
            ViewName = "Error",
            StatusCode = StatusCodes.Status503ServiceUnavailable
        };

        context.ExceptionHandled = true;
    }

    // Decides whether the exception comes from talking to obilet.
    private static bool IsObiletFailure(Exception exception)
    {
        // obilet answered with an error status.
        if (exception is ObiletApiException)
        {
            return true;
        }

        // obilet could not be reached or refused the request, e.g. when too many requests were sent.
        if (exception is HttpRequestException)
        {
            return true;
        }

        // obilet did not answer in time. HttpClient marks its own timeout with an inner TimeoutException;
        // without it the visitor cancelled the request, which is not an error.
        if (exception is TaskCanceledException)
        {
            return exception.InnerException is TimeoutException;
        }

        return false;
    }
}
