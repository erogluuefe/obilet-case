using Microsoft.Extensions.Options;
using ObiletCase.Web.Configuration;
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

    // Two records from a real GetBusLocations response, with most of the 31 fields left out.
    private const string LocationsJson = """
        {
          "status": "Success",
          "data": [
            {
              "id": 349,
              "parent-id": 250,
              "type": "Town",
              "name": "İstanbul Avrupa",
              "geo-location": { "latitude": 41.02854, "longitude": 28.92315, "zoom": 12 },
              "rank": 1,
              "city-name": "İstanbul",
              "long-name": "İstanbul Avrupa - İstanbul"
            },
            {
              "id": 350,
              "parent-id": 250,
              "type": "Town",
              "name": "İstanbul Anadolu",
              "geo-location": { "latitude": 40.99, "longitude": 29.03, "zoom": 12 },
              "rank": 2,
              "city-name": "İstanbul",
              "long-name": "İstanbul Anadolu - İstanbul"
            }
          ],
          "message": null,
          "user-message": null,
          "controller": "LocationController"
        }
        """;

    // Two records from a real GetJourneys response, with most of the 100+ fields left out.
    private const string JourneysJson = """
        {
          "status": "Success",
          "data": [
            {
              "id": 1405905513,
              "partner-id": 32325,
              "partner-name": "Yükseller Aksaray Birlik",
              "bus-type": "2+1",
              "total-seats": 41,
              "available-seats": 38,
              "origin-location": "İstanbul Avrupa",
              "destination-location": "Ankara",
              "journey": {
                "kind": "Bus",
                "code": "262818-1332-1274",
                "origin": "Alibeyköy Otogarı",
                "destination": "Ankara (Aşti) Otogarı",
                "departure": "2026-10-12T09:15:00",
                "arrival": "2026-10-12T16:30:00",
                "currency": "TRY",
                "duration": "07:15:00",
                "original-price": 600.0000,
                "internet-price": 545.0000
              }
            },
            {
              "id": 1405905470,
              "partner-id": 32325,
              "partner-name": "Yükseller Aksaray Birlik",
              "bus-type": "2+1",
              "total-seats": 41,
              "available-seats": 38,
              "origin-location": "İstanbul Avrupa",
              "destination-location": "Ankara",
              "journey": {
                "kind": "Bus",
                "code": "262818-1332-1274",
                "origin": "Esenler Otogarı",
                "destination": "Ankara (Aşti) Otogarı",
                "departure": "2026-10-12T09:00:00",
                "arrival": "2026-10-12T16:30:00",
                "currency": "TRY",
                "duration": "07:30:00",
                "original-price": 600.0000,
                "internet-price": 545.0000
              }
            }
          ],
          "message": null,
          "user-message": null,
          "controller": "JourneyController"
        }
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

    [Fact]
    public async Task GetBusLocationsAsync_WithoutSearchText_PostsNullDataWithTheVisitorSession()
    {
        var handler = new FakeHttpMessageHandler(LocationsJson);
        var client = CreateClient(handler);

        await client.GetBusLocationsAsync(CreateSession(), searchText: null);

        Assert.Equal("https://example.test/api/location/getbuslocations", handler.LastRequest!.RequestUri!.ToString());
        Assert.Contains("\"data\":null", handler.LastRequestBody);
        Assert.Contains("\"device-session\":{\"session-id\":\"session-1\",\"device-id\":\"device-1\"}", handler.LastRequestBody);
        Assert.Contains("\"language\":\"tr-TR\"", handler.LastRequestBody);
    }

    [Fact]
    public async Task GetBusLocationsAsync_WithSearchText_PostsItAsData()
    {
        var handler = new FakeHttpMessageHandler(LocationsJson);
        var client = CreateClient(handler);

        await client.GetBusLocationsAsync(CreateSession(), "ank");

        Assert.Contains("\"data\":\"ank\"", handler.LastRequestBody);
    }

    [Fact]
    public async Task GetBusLocationsAsync_SendsTheDateWithoutFractionsOrTimeZone()
    {
        var handler = new FakeHttpMessageHandler(LocationsJson);
        var client = CreateClient(handler);

        await client.GetBusLocationsAsync(CreateSession(), searchText: null);

        // e.g. "date":"2026-10-11T12:40:00"
        Assert.Matches("\"date\":\"\\d{4}-\\d{2}-\\d{2}T\\d{2}:\\d{2}:\\d{2}\"", handler.LastRequestBody);
    }

    [Fact]
    public async Task GetBusLocationsAsync_ReturnsLocationsInTheOrderObiletSentThem()
    {
        var handler = new FakeHttpMessageHandler(LocationsJson);
        var client = CreateClient(handler);

        var locations = await client.GetBusLocationsAsync(CreateSession(), searchText: null);

        Assert.Equal(2, locations.Count);
        Assert.Equal(349, locations[0].Id);
        Assert.Equal("İstanbul Avrupa", locations[0].Name);
        Assert.Equal(350, locations[1].Id);
        Assert.Equal("İstanbul Anadolu", locations[1].Name);
    }

    [Fact]
    public async Task GetBusJourneysAsync_PostsTheRouteAndDayWithTheVisitorSession()
    {
        var handler = new FakeHttpMessageHandler(JourneysJson);
        var client = CreateClient(handler);

        await client.GetBusJourneysAsync(CreateSession(), originId: 349, destinationId: 356, new DateOnly(2026, 10, 12));

        Assert.Equal("https://example.test/api/journey/getbusjourneys", handler.LastRequest!.RequestUri!.ToString());
        Assert.Contains("\"data\":{\"origin-id\":349,\"destination-id\":356,\"departure-date\":\"2026-10-12\"}", handler.LastRequestBody);
        Assert.Contains("\"device-session\":{\"session-id\":\"session-1\",\"device-id\":\"device-1\"}", handler.LastRequestBody);
        Assert.Contains("\"language\":\"tr-TR\"", handler.LastRequestBody);
    }

    [Fact]
    public async Task GetBusJourneysAsync_SendsTheRequestDateWithoutATimePart()
    {
        var handler = new FakeHttpMessageHandler(JourneysJson);
        var client = CreateClient(handler);

        await client.GetBusJourneysAsync(CreateSession(), originId: 349, destinationId: 356, new DateOnly(2026, 10, 12));

        // e.g. "date":"2026-10-11"
        Assert.Matches("\"date\":\"\\d{4}-\\d{2}-\\d{2}\"", handler.LastRequestBody);
    }

    [Fact]
    public async Task GetBusJourneysAsync_ReturnsTheFieldsTheJourneyPageShows()
    {
        var handler = new FakeHttpMessageHandler(JourneysJson);
        var client = CreateClient(handler);

        var journeys = await client.GetBusJourneysAsync(CreateSession(), originId: 349, destinationId: 356, new DateOnly(2026, 10, 12));

        Assert.Equal(2, journeys.Count);

        var first = journeys[0];
        Assert.Equal("Yükseller Aksaray Birlik", first.PartnerName);
        Assert.Equal("İstanbul Avrupa", first.OriginLocation);
        Assert.Equal("Ankara", first.DestinationLocation);
        Assert.Equal("Alibeyköy Otogarı", first.Journey.Origin);
        Assert.Equal("Ankara (Aşti) Otogarı", first.Journey.Destination);
        Assert.Equal(new DateTime(2026, 10, 12, 9, 15, 0), first.Journey.Departure);
        Assert.Equal(new DateTime(2026, 10, 12, 16, 30, 0), first.Journey.Arrival);
        Assert.Equal(545m, first.Journey.InternetPrice);
        Assert.Equal("TRY", first.Journey.Currency);
    }

    private static ObiletApiClient CreateClient(FakeHttpMessageHandler handler)
    {
        var httpClient = new HttpClient(handler);
        httpClient.BaseAddress = new Uri("https://example.test/api/");

        var options = new ObiletApiOptions
        {
            BaseUrl = "https://example.test/api/",
            ClientToken = "test-token",
            Language = "tr-TR"
        };

        return new ObiletApiClient(httpClient, Options.Create(options));
    }

    private static DeviceSession CreateSession()
    {
        return new DeviceSession { SessionId = "session-1", DeviceId = "device-1" };
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
