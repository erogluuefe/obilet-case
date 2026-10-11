using ObiletCase.Web.Services.Obilet;
using ObiletCase.Web.Services.Obilet.Models;

namespace ObiletCase.Tests;

/// <summary>Stands in for the session service: always returns the same visitor session.</summary>
public class FakeObiletSessionService : IObiletSessionService
{
    public DeviceSession Session { get; } = new DeviceSession { SessionId = "visitor-session", DeviceId = "visitor-device" };

    public Task<DeviceSession> GetSessionAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(Session);
    }
}
