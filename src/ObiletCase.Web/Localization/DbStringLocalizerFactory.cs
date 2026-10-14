using Microsoft.Extensions.Localization;

namespace ObiletCase.Web.Localization;

/// <summary>Creates localizers that read from the translation store.</summary>
public class DbStringLocalizerFactory : IStringLocalizerFactory
{
    private readonly ITranslationStore _translationStore;

    public DbStringLocalizerFactory(ITranslationStore translationStore)
    {
        _translationStore = translationStore;
    }

    // All texts live in one table, so which view or model asks does not matter.
    public IStringLocalizer Create(Type resourceSource)
    {
        return new DbStringLocalizer(_translationStore);
    }

    public IStringLocalizer Create(string baseName, string location)
    {
        return new DbStringLocalizer(_translationStore);
    }
}
