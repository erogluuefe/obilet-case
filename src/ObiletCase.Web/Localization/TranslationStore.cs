using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using ObiletCase.Web.Configuration;
using ObiletCase.Web.Data;

namespace ObiletCase.Web.Localization;

/// <summary>Reads translations from the database and keeps each language in memory for a while.</summary>
public class TranslationStore : ITranslationStore
{
    private readonly IDbContextFactory<AppDbContext> _dbContextFactory;
    private readonly IMemoryCache _cache;

    // How long a language stays cached is set in appsettings ("Translations" section).
    private readonly TimeSpan _cacheDuration;

    public TranslationStore(IDbContextFactory<AppDbContext> dbContextFactory, IMemoryCache cache, IOptions<TranslationOptions> options)
    {
        _dbContextFactory = dbContextFactory;
        _cache = cache;
        _cacheDuration = TimeSpan.FromMinutes(options.Value.CacheMinutes);
    }

    public IReadOnlyDictionary<string, string> GetTranslations(string culture)
    {
        var translations = _cache.GetOrCreate("translations:" + culture, entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = _cacheDuration;

            // This class is a singleton, so it creates a short-lived DbContext for each read.
            using var database = _dbContextFactory.CreateDbContext();

            return database.Translations
                .AsNoTracking()
                .Where(t => t.Culture == culture)
                .ToDictionary(t => t.Key, t => t.Value);
        });

        if (translations == null)
        {
            return new Dictionary<string, string>();
        }

        return translations;
    }
}
