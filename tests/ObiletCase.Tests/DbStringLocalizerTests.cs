using System.Globalization;
using ObiletCase.Web.Localization;

namespace ObiletCase.Tests;

public class DbStringLocalizerTests : IDisposable
{
    private readonly CultureInfo _originalCulture = CultureInfo.CurrentCulture;
    private readonly CultureInfo _originalUiCulture = CultureInfo.CurrentUICulture;
    private readonly FakeTranslationStore _store = new();

    public DbStringLocalizerTests()
    {
        _store.TranslationsByCulture["tr-TR"] = new Dictionary<string, string>
        {
            ["Greeting"] = "Merhaba {0}"
        };
        _store.TranslationsByCulture["en-US"] = new Dictionary<string, string>
        {
            ["Greeting"] = "Hello {0}"
        };
    }

    public void Dispose()
    {
        CultureInfo.CurrentCulture = _originalCulture;
        CultureInfo.CurrentUICulture = _originalUiCulture;
    }

    [Fact]
    public void Indexer_ReturnsTranslationOfCurrentUiCulture()
    {
        var localizer = new DbStringLocalizer(_store);

        CultureInfo.CurrentUICulture = new CultureInfo("tr-TR");
        var turkish = localizer["Greeting"];

        CultureInfo.CurrentUICulture = new CultureInfo("en-US");
        var english = localizer["Greeting"];

        Assert.Equal("Merhaba {0}", turkish.Value);
        Assert.Equal("Hello {0}", english.Value);
        Assert.False(english.ResourceNotFound);
    }

    [Fact]
    public void Indexer_ReturnsKeyWhenTranslationIsMissing()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("en-US");
        var localizer = new DbStringLocalizer(_store);

        var result = localizer["Unknown.Key"];

        Assert.Equal("Unknown.Key", result.Value);
        Assert.True(result.ResourceNotFound);
    }

    [Fact]
    public void Indexer_WithArguments_FillsInArguments()
    {
        CultureInfo.CurrentCulture = new CultureInfo("en-US");
        CultureInfo.CurrentUICulture = new CultureInfo("en-US");
        var localizer = new DbStringLocalizer(_store);

        var result = localizer["Greeting", "Efe"];

        Assert.Equal("Hello Efe", result.Value);
        Assert.False(result.ResourceNotFound);
    }

    [Fact]
    public void Factory_CreatesLocalizersThatReadFromTheStore()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("en-US");
        var factory = new DbStringLocalizerFactory(_store);

        var fromType = factory.Create(typeof(DbStringLocalizerTests));
        var fromName = factory.Create("x", "y");

        Assert.Equal("Hello {0}", fromType["Greeting"].Value);
        Assert.Equal("Hello {0}", fromName["Greeting"].Value);
    }
}
