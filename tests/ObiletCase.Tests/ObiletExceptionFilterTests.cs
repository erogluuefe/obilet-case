using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging.Abstractions;
using ObiletCase.Web.Filters;
using ObiletCase.Web.Services.Obilet;

namespace ObiletCase.Tests;

public class ObiletExceptionFilterTests
{
    [Fact]
    public void ErrorAnswerFromObilet_ShowsTheErrorPage()
    {
        var context = CreateContext(new ObiletApiException("Timeout", "Operation has timed out.", null));

        CreateFilter().OnException(context);

        var viewResult = Assert.IsType<ViewResult>(context.Result);
        Assert.Equal("Error", viewResult.ViewName);
        Assert.Equal(503, viewResult.StatusCode);
        Assert.True(context.ExceptionHandled);
    }

    [Fact]
    public void ObiletCannotBeReached_ShowsTheErrorPage()
    {
        var context = CreateContext(new HttpRequestException("Response status code does not indicate success: 429."));

        CreateFilter().OnException(context);

        var viewResult = Assert.IsType<ViewResult>(context.Result);
        Assert.Equal("Error", viewResult.ViewName);
        Assert.True(context.ExceptionHandled);
    }

    [Fact]
    public void ObiletDoesNotAnswerInTime_ShowsTheErrorPage()
    {
        // This is how HttpClient reports its own timeout.
        var timeout = new TaskCanceledException("The request was canceled due to the configured HttpClient.Timeout.", new TimeoutException());
        var context = CreateContext(timeout);

        CreateFilter().OnException(context);

        var viewResult = Assert.IsType<ViewResult>(context.Result);
        Assert.Equal("Error", viewResult.ViewName);
        Assert.True(context.ExceptionHandled);
    }

    [Fact]
    public void VisitorCancelsTheRequest_IsNotTreatedAsAnObiletFailure()
    {
        var context = CreateContext(new TaskCanceledException("The operation was canceled."));

        CreateFilter().OnException(context);

        Assert.Null(context.Result);
        Assert.False(context.ExceptionHandled);
    }

    [Fact]
    public void OtherException_IsLeftToTheNormalErrorHandling()
    {
        var context = CreateContext(new InvalidOperationException("a bug"));

        CreateFilter().OnException(context);

        Assert.Null(context.Result);
        Assert.False(context.ExceptionHandled);
    }

    private static ObiletExceptionFilter CreateFilter()
    {
        return new ObiletExceptionFilter(NullLogger<ObiletExceptionFilter>.Instance);
    }

    private static ExceptionContext CreateContext(Exception exception)
    {
        var actionContext = new ActionContext(new DefaultHttpContext(), new RouteData(), new ActionDescriptor());
        var context = new ExceptionContext(actionContext, new List<IFilterMetadata>());
        context.Exception = exception;

        return context;
    }
}
