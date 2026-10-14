using ObiletCase.Web.Localization;

namespace ObiletCase.Tests;

/// <summary>Stands in for the translation store: returns the prepared texts of each culture.</summary>
public class FakeTranslationStore : ITranslationStore
{
    public Dictionary<string, Dictionary<string, string>> TranslationsByCulture { get; } = new();

    public IReadOnlyDictionary<string, string> GetTranslations(string culture)
    {
        if (TranslationsByCulture.TryGetValue(culture, out var translations))
        {
            return translations;
        }

        return new Dictionary<string, string>();
    }
}
