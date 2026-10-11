namespace ObiletCase.Web.Services;

/// <summary>The current date and time in Turkey, which does not depend on the time zone of the server.</summary>
public static class TurkeyTime
{
    // The zone is looked up by name instead of adding three hours by hand, so the offset comes from the time zone data.
    // The machine needs that data to be installed, which is the default on Windows, macOS and regular Linux images.
    private static readonly TimeZoneInfo TimeZone = TimeZoneInfo.FindSystemTimeZoneById("Europe/Istanbul");

    public static DateTime Now
    {
        get
        {
            // UTC is the same moment on every machine; only the conversion to Turkey's clock is applied to it.
            return TimeZoneInfo.ConvertTime(DateTime.UtcNow, TimeZone);
        }
    }

    public static DateOnly Today
    {
        get
        {
            return DateOnly.FromDateTime(Now);
        }
    }
}
