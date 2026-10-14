using System.Globalization;
using Microsoft.Extensions.Localization;

namespace ObiletCase.Web.Localization;

/// <summary>Looks up texts for the current UI culture in the translation store.</summary>
public class DbStringLocalizer : IStringLocalizer
{
    private readonly ITranslationStore _translationStore;

    public DbStringLocalizer(ITranslationStore translationStore)
    {
        _translationStore = translationStore;
    }

    public LocalizedString this[string name]
    {
        get
        {
            // The culture is read on every call because MVC may reuse this instance across requests.
            var culture = CultureInfo.CurrentUICulture.Name;
            var translations = _translationStore.GetTranslations(culture);

            if (translations.TryGetValue(name, out var value))
            {
                return new LocalizedString(name, value, resourceNotFound: false);
            }

            // Showing the key is better than breaking the page when a translation is missing.
            return new LocalizedString(name, name, resourceNotFound: true);
        }
    }

    public LocalizedString this[string name, params object[] arguments]
    {
        get
        {
            var localized = this[name];
            var formattedValue = string.Format(CultureInfo.CurrentCulture, localized.Value, arguments);

            return new LocalizedString(name, formattedValue, localized.ResourceNotFound);
        }
    }

    public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
    {
        var culture = CultureInfo.CurrentUICulture.Name;
        var translations = _translationStore.GetTranslations(culture);

        foreach (var translation in translations)
        {
            yield return new LocalizedString(translation.Key, translation.Value, resourceNotFound: false);
        }
    }
}
