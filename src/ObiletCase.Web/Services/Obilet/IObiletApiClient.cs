using ObiletCase.Web.Services.Obilet.Models;

namespace ObiletCase.Web.Services.Obilet;

public interface IObiletApiClient
{
    Task<DeviceSession> GetSessionAsync(GetSessionRequest request, CancellationToken cancellationToken = default);
}
