using LangDetect.Models;

namespace LangDetect.Utility;

/// <summary>
/// Maps between the <see cref="Language"/> enum and ISO 639-1 language codes.
/// Used to populate <see cref="DetectionResult.IsoCode"/>.
/// </summary>
public static class LanguageCode
{
    /// <summary>
    /// Returns the ISO 639-1 code for <paramref name="language"/>.
    /// Returns "und" (ISO 639-3 undetermined) for
    /// <see cref="Language.Unknown"/> or any unrecognized value.
    /// </summary>
    public static string ToIso(Language language) => language switch
    {
        Language.English => "en",
        Language.Hindi => "hi",
        Language.Arabic => "ar",
        Language.Mandarin => "zh",
        Language.Japanese => "ja",
        Language.Korean => "ko",
        Language.Sinhala => "si",
        Language.Tamil => "ta",
        _ => "und",
    };

    /// <summary>
    /// Returns the <see cref="Language"/> for a given ISO 639-1
    /// <paramref name="code"/>. Case-insensitive.
    /// Returns <see cref="Language.Unknown"/> for unrecognized codes.
    /// </summary>
    public static Language FromIso(string code) => code.ToLowerInvariant() switch
    {
        "en" => Language.English,
        "hi" => Language.Hindi,
        "ar" => Language.Arabic,
        "zh" => Language.Mandarin,
        "ja" => Language.Japanese,
        "ko" => Language.Korean,
        "si" => Language.Sinhala,
        "ta" => Language.Tamil,
        _ => Language.Unknown,
    };
}