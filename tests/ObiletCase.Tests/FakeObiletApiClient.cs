using ObiletCase.Web.Services.Obilet;
using ObiletCase.Web.Services.Obilet.Models;

namespace ObiletCase.Tests;

/// <summary>Stands in for the obilet API client: records what it was asked and returns the prepared answers.</summary>
public class FakeObiletApiClient : IObiletApiClient
{
    public int GetSessionCallCount { get; private set; }
    public GetSessionRequest? LastRequest { get; private set; }

    public List<BusLocation> Locations { get; set; } = new();
    public DeviceSession? LastLocationsSession { get; private set; }
    public string? LastSearchText { get; private set; }

    public List<BusJourney> Journeys { get; set; } = new();
    public DeviceSession? LastJourneysSession { get; private set; }
    public int LastOriginId { get; private set; }
    public int LastDestinationId { get; private set; }
    public DateOnly LastDepartureDate { get; private set; }

    public Task<DeviceSession> GetSessionAsync(GetSessionRequest request, CancellationToken cancellationToken = default)
    {
        GetSessionCallCount++;
        LastRequest = request;

        var deviceSession = new DeviceSession
        {
            SessionId = "session-" + GetSessionCallCount,
            DeviceId = "device-" + GetSessionCallCount
        };

        return Task.FromResult(deviceSession);
    }

    public Task<List<BusLocation>> GetBusLocationsAsync(DeviceSession session, string? searchText, CancellationToken cancellationToken = default)
    {
        LastLocationsSession = session;
        LastSearchText = searchText;

        return Task.FromResult(Locations);
    }

    public Task<List<BusJourney>> GetBusJourneysAsync(DeviceSession session, int originId, int destinationId, DateOnly departureDate, CancellationToken cancellationToken = default)
    {
        LastJourneysSession = session;
        LastOriginId = originId;
        LastDestinationId = destinationId;
        LastDepartureDate = departureDate;

        return Task.FromResult(Journeys);
    }
}
