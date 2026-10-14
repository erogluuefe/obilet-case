using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using ObiletCase.Web.Configuration;
using ObiletCase.Web.Data;
using ObiletCase.Web.Localization;

namespace ObiletCase.Tests;

public class TranslationStoreTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly DbContextOptions<AppDbContext> _options;
    private readonly MemoryCache _cache;
    private readonly TranslationStore _store;

    public TranslationStoreTests()
    {
        // An in-memory SQLite database lives only as long as its connection, so the connection stays open for the test.
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        _options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(_connection)
            .Options;

        // EnsureCreated also inserts the seeded translations.
        using (var database = new AppDbContext(_options))
        {
            database.Database.EnsureCreated();
        }

        _cache = new MemoryCache(new MemoryCacheOptions());
        _store = new TranslationStore(new FakeDbContextFactory(_options), _cache, Options.Create(new LocalizationOptions()));
    }

    public void Dispose()
    {
        _cache.Dispose();
        _connection.Dispose();
    }

    [Fact]
    public void GetTranslations_ReturnsOnlyTheRequestedCulture()
    {
        var english = _store.GetTranslations("en-US");
        var turkish = _store.GetTranslations("tr-TR");

        Assert.Equal("Find Tickets", english["FindTickets"]);
        Assert.Equal("Bileti Bul", turkish["FindTickets"]);
    }

    [Fact]
    public void GetTranslations_ReturnsEmptyDictionary_ForUnknownCulture()
    {
        var translations = _store.GetTranslations("xx-XX");

        Assert.Empty(translations);
    }

    [Fact]
    public void GetTranslations_ServesSecondCallFromCache()
    {
        _store.GetTranslations("en-US");

        using (var database = new AppDbContext(_options))
        {
            database.Translations.ExecuteDelete();
        }

        var translations = _store.GetTranslations("en-US");

        Assert.Equal("Find Tickets", translations["FindTickets"]);
    }
}
