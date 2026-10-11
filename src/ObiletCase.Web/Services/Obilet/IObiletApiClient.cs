using ObiletCase.Web.Services.Obilet.Models;

namespace ObiletCase.Web.Services.Obilet;

/// <summary>Sends requests to the obilet.com business API.</summary>
public interface IObiletApiClient
{
    /// <summary>Asks obilet for a new session for the given visitor.</summary>
    Task<DeviceSession> GetSessionAsync(GetSessionRequest request, CancellationToken cancellationToken = default);

    /// <summary>Returns the locations matching the search text, or obilet's default list when it is null.</summary>
    Task<List<BusLocation>> GetBusLocationsAsync(DeviceSession session, string? searchText, CancellationToken cancellationToken = default);
}
