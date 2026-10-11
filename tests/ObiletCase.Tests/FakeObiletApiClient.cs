using ObiletCase.Web.Services.Obilet;
using ObiletCase.Web.Services.Obilet.Models;

namespace ObiletCase.Tests;

/// <summary>Stands in for the obilet API client: counts the calls and returns a new session for each one.</summary>
public class FakeObiletApiClient : IObiletApiClient
{
    public int GetSessionCallCount { get; private set; }
    public GetSessionRequest? LastRequest { get; private set; }

    public List<BusLocation> Locations { get; set; } = new();
    public DeviceSession? LastLocationsSession { get; private set; }
    public string? LastSearchText { get; private set; }

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
}
