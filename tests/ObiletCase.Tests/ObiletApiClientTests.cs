using ObiletCase.Web.Services.Obilet;
using ObiletCase.Web.Services.Obilet.Models;

namespace ObiletCase.Tests;

public class ObiletApiClientTests
{
    // Shape of a real GetSession response; only the ids are replaced with dummy values.
    private const string SuccessJson = """
        {
          "status": "Success",
          "data": {
            "session-id": "session-1",
            "device-id": "device-1",
            "affiliate": null,
            "device-type": 0,
            "device": null,
            "ip-country": "TR",
            "clean-session-id": 3833736426,
            "clean-device-id": -1863326116,
            "ip-address": null,
            "browser-type": null,
            "operating-system": null
          },
          "message": null,
          "user-message": null,
          "api-request-id": null,
          "controller": "ClientController",
          "client-request-id": null,
          "web-correlation-id": null,
          "correlation-id": "49e6be18-cda1-4422-8ac3-3419a4d4cc8b",
          "parameters": null
        }
        """;

    private const string TimeoutJson = """
        { "status": "Timeout", "data": null, "message": "Operation has timed out.", "user-message": "Please try again." }
        """;

    [Fact]
    public async Task GetSessionAsync_PostsKebabCaseBodyToGetSessionEndpoint()
    {
        var handler = new FakeHttpMessageHandler(SuccessJson);
        var client = CreateClient(handler);

        await client.GetSessionAsync(CreateRequest());

        Assert.Equal(HttpMethod.Post, handler.LastRequest!.Method);
        Assert.Equal("https://example.test/api/client/getsession", handler.LastRequest.RequestUri!.ToString());
        Assert.Contains("\"ip-address\":\"1.2.3.4\"", handler.LastRequestBody);
    }

    [Fact]
    public async Task GetSessionAsync_ReturnsSessionAndDeviceIds()
    {
        var handler = new FakeHttpMessageHandler(SuccessJson);
        var client = CreateClient(handler);

        var session = await client.GetSessionAsync(CreateRequest());

        Assert.Equal("session-1", session.SessionId);
        Assert.Equal("device-1", session.DeviceId);
    }

    [Fact]
    public async Task GetSessionAsync_ThrowsWhenStatusIsNotSuccess()
    {
        var handler = new FakeHttpMessageHandler(TimeoutJson);
        var client = CreateClient(handler);

        var exception = await Assert.ThrowsAsync<ObiletApiException>(() => client.GetSessionAsync(CreateRequest()));

        Assert.Equal("Timeout", exception.Status);
        Assert.Equal("Please try again.", exception.UserMessage);
    }

    private static ObiletApiClient CreateClient(FakeHttpMessageHandler handler)
    {
        var httpClient = new HttpClient(handler);
        httpClient.BaseAddress = new Uri("https://example.test/api/");
        return new ObiletApiClient(httpClient);
    }

    private static GetSessionRequest CreateRequest()
    {
        return new GetSessionRequest
        {
            Connection = new SessionConnection { IpAddress = "1.2.3.4", Port = "5117" },
            Browser = new SessionBrowser { Name = "Chrome", Version = "47.0" }
        };
    }
}
