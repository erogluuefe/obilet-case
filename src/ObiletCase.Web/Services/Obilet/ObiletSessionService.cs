using ObiletCase.Web.Services.Obilet.Models;
using UAParser;

namespace ObiletCase.Web.Services.Obilet;

/// <summary>Gives every visitor their own obilet session and reuses it for their later requests.</summary>
public class ObiletSessionService : IObiletSessionService
{
    private const string SessionIdKey = "ObiletSessionId";
    private const string DeviceIdKey = "ObiletDeviceId";

    // Loading the User-Agent parsing rules is expensive, so one parser is shared by all requests.
    private static readonly Parser UserAgentParser = Parser.GetDefault();

    private readonly IObiletApiClient _apiClient;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ObiletSessionService(IObiletApiClient apiClient, IHttpContextAccessor httpContextAccessor)
    {
        _apiClient = apiClient;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<DeviceSession> GetSessionAsync(CancellationToken cancellationToken = default)
    {
        var httpContext = _httpContextAccessor.HttpContext;

        if (httpContext == null)
        {
            throw new InvalidOperationException("An obilet session can only be requested during an HTTP request.");
        }

        var sessionId = httpContext.Session.GetString(SessionIdKey);
        var deviceId = httpContext.Session.GetString(DeviceIdKey);

        // The visitor already has an obilet session, so no API call is needed.
        if (!string.IsNullOrEmpty(sessionId) && !string.IsNullOrEmpty(deviceId))
        {
            return new DeviceSession { SessionId = sessionId, DeviceId = deviceId };
        }

        var request = CreateSessionRequest(httpContext);
        var deviceSession = await _apiClient.GetSessionAsync(request, cancellationToken);

        httpContext.Session.SetString(SessionIdKey, deviceSession.SessionId);
        httpContext.Session.SetString(DeviceIdKey, deviceSession.DeviceId);

        return deviceSession;
    }

    private static GetSessionRequest CreateSessionRequest(HttpContext httpContext)
    {
        var ipAddress = "0.0.0.0";

        if (httpContext.Connection.RemoteIpAddress != null)
        {
            ipAddress = httpContext.Connection.RemoteIpAddress.ToString();
        }

        // obilet expects a short browser name and version, not the full User-Agent text.
        var userAgent = httpContext.Request.Headers.UserAgent.ToString();
        var browser = UserAgentParser.Parse(userAgent).UA;

        var majorVersion = "0";

        if (!string.IsNullOrEmpty(browser.Major))
        {
            majorVersion = browser.Major;
        }

        var minorVersion = "0";

        if (!string.IsNullOrEmpty(browser.Minor))
        {
            minorVersion = browser.Minor;
        }

        return new GetSessionRequest
        {
            Connection = new SessionConnection
            {
                IpAddress = ipAddress,
                Port = httpContext.Connection.RemotePort.ToString()
            },
            Browser = new SessionBrowser
            {
                Name = browser.Family,
                Version = majorVersion + "." + minorVersion
            }
        };
    }
}
