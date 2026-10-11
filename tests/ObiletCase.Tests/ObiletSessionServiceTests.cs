using System.Net;
using Microsoft.AspNetCore.Http;
using ObiletCase.Web.Services.Obilet;

namespace ObiletCase.Tests;

public class ObiletSessionServiceTests
{
    private const string ChromeUserAgent =
        "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/129.0.0.0 Safari/537.36";

    [Fact]
    public async Task GetSessionAsync_FirstRequest_GetsSessionFromObilet()
    {
        var apiClient = new FakeObiletApiClient();
        var visitor = CreateVisitor(ChromeUserAgent);
        var service = CreateService(apiClient, visitor);

        var session = await service.GetSessionAsync();

        Assert.Equal(1, apiClient.GetSessionCallCount);
        Assert.Equal("session-1", session.SessionId);
        Assert.Equal("device-1", session.DeviceId);
    }

    [Fact]
    public async Task GetSessionAsync_SameVisitorAgain_ReusesSessionWithoutCallingObilet()
    {
        var apiClient = new FakeObiletApiClient();
        var visitor = CreateVisitor(ChromeUserAgent);
        var service = CreateService(apiClient, visitor);

        await service.GetSessionAsync();
        var secondSession = await service.GetSessionAsync();

        Assert.Equal(1, apiClient.GetSessionCallCount);
        Assert.Equal("session-1", secondSession.SessionId);
        Assert.Equal("device-1", secondSession.DeviceId);
    }

    [Fact]
    public async Task GetSessionAsync_DifferentVisitors_GetDifferentSessions()
    {
        var apiClient = new FakeObiletApiClient();
        var firstVisitorService = CreateService(apiClient, CreateVisitor(ChromeUserAgent));
        var secondVisitorService = CreateService(apiClient, CreateVisitor(ChromeUserAgent));

        var firstSession = await firstVisitorService.GetSessionAsync();
        var secondSession = await secondVisitorService.GetSessionAsync();

        Assert.Equal(2, apiClient.GetSessionCallCount);
        Assert.NotEqual(firstSession.SessionId, secondSession.SessionId);
    }

    [Fact]
    public async Task GetSessionAsync_SendsVisitorConnectionAndBrowserToObilet()
    {
        var apiClient = new FakeObiletApiClient();
        var visitor = CreateVisitor(ChromeUserAgent);
        var service = CreateService(apiClient, visitor);

        await service.GetSessionAsync();

        var request = apiClient.LastRequest!;
        Assert.Equal("10.1.2.3", request.Connection.IpAddress);
        Assert.Equal("5117", request.Connection.Port);
        Assert.Equal("Chrome", request.Browser.Name);
        Assert.Equal("129.0", request.Browser.Version);
    }

    [Fact]
    public async Task GetSessionAsync_WithoutUserAgent_StillGetsSession()
    {
        var apiClient = new FakeObiletApiClient();
        var visitor = CreateVisitor(userAgent: null);
        var service = CreateService(apiClient, visitor);

        var session = await service.GetSessionAsync();

        Assert.Equal("session-1", session.SessionId);
        Assert.Equal("Other", apiClient.LastRequest!.Browser.Name);
        Assert.Equal("0.0", apiClient.LastRequest.Browser.Version);
    }

    // Every visitor is an HttpContext with its own, empty ASP.NET session.
    private static HttpContext CreateVisitor(string? userAgent)
    {
        var httpContext = new DefaultHttpContext();
        httpContext.Session = new FakeSession();
        httpContext.Connection.RemoteIpAddress = IPAddress.Parse("10.1.2.3");
        httpContext.Connection.RemotePort = 5117;

        if (userAgent != null)
        {
            httpContext.Request.Headers.UserAgent = userAgent;
        }

        return httpContext;
    }

    private static ObiletSessionService CreateService(FakeObiletApiClient apiClient, HttpContext visitor)
    {
        var httpContextAccessor = new FakeHttpContextAccessor();
        httpContextAccessor.HttpContext = visitor;

        return new ObiletSessionService(apiClient, httpContextAccessor);
    }

    /// <summary>The real HttpContextAccessor shares one static slot, so two visitors in one test would overwrite each other.</summary>
    private class FakeHttpContextAccessor : IHttpContextAccessor
    {
        public HttpContext? HttpContext { get; set; }
    }
}
