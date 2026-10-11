using ObiletCase.Web.Services;

namespace ObiletCase.Tests;

public class TurkeyTimeTests
{
    [Fact]
    public void Now_IsThreeHoursAheadOfUtc()
    {
        var utcNow = DateTime.UtcNow;

        var difference = TurkeyTime.Now - utcNow;

        // Turkey has been on UTC+3 all year since 2016. The test guards that assumption and the zone name;
        // a few seconds are allowed for the time between the two readings.
        Assert.InRange(difference.TotalMinutes, 179, 181);
    }
}
