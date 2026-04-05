namespace LangDetect.Models;

/// <summary>
/// List of supported languages by the engine.
/// </summary>
public enum Language
{
    /// <summary>
    /// Language could not be identified with sufficient confidence.
    /// ISO 639-3 language code: und (undetermined)
    /// </summary>
    Unknown = 0,

    /// <summary>
    /// English — Latin script (U+0000–U+007F).
    /// ISO 639-1 language code: en
    /// </summary>
    English = 1,

    /// <summary>
    /// Devanagari script (U+0900–U+097F).
    /// ISO 639-1 language code: hi
    /// </summary>
    Hindi = 2,

    /// <summary>
    /// Arabic script (U+0600–U+06FF).
    /// ISO 639-1 language code: ar
    /// </summary>
    Arabic = 3,

    /// <summary>
    /// CJK Unified Ideographs (U+4E00–U+9FFF).
    /// ISO 639-1 language code: zh
    /// </summary>
    Mandarin = 4,

    /// <summary>
    /// todo: Japanese — Hiragana (U+3040–U+309F) and Katakana (U+30A0–U+30FF) = pure japanese unicode
    /// todo: logic: hiragana || katakana && Chinese = mixed japanese unicode.
    /// ISO 639-1 language code: ja
    /// </summary>
    Japanese = 5,

    /// <summary>
    /// Hangul Syllables (U+AC00–U+D7AF).
    /// ISO 639-1 language code: ko
    /// </summary>
    Korean = 6,

    /// <summary>
    /// Sinhala script (U+0D80–U+0DFF).
    /// ISO 639-1 language code: si
    /// </summary>
    Sinhala = 7,

    /// <summary>
    /// Tamil script (U+0B80–U+0BFF).
    /// ISO 639-1 language code: ta
    /// </summary>
    Tamil = 8,
}