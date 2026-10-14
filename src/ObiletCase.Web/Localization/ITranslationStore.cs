namespace ObiletCase.Web.Localization;

/// <summary>Gives all texts of a language, keyed by translation key.</summary>
public interface ITranslationStore
{
    IReadOnlyDictionary<string, string> GetTranslations(string culture);
}
