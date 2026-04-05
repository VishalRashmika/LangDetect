using LangDetect.Models;

namespace LangDetect.Abstractions;

/// <summary>
/// Defines a word list used by <c>CommonWordDetectionStage</c>
/// for frequency-based language detection.
/// All entries must be romanized Latin script — no Unicode characters.
/// </summary>
public interface IWordList
{
    /// <summary>
    /// The language this word list identifies.
    /// For Singlish and Tanglish word lists this still returns
    /// <see cref="Language.Sinhala"/> and <see cref="Language.Tamil"/>
    /// respectively — romanized script is not a separate language.
    /// </summary>
    Language Language { get; }

    /// <summary>
    /// The set of words used for frequency matching.
    /// All entries are lowercase romanized Latin.
    /// Loaded from an embedded .txt resource at construction time.
    /// </summary>
    IReadOnlySet<string> Words { get; }

    /// <summary>
    /// Minimum ratio of input tokens that must match <see cref="Words"/>
    /// for this list to produce a confident result. Range: 0.0 – 1.0.
    /// </summary>
    float MinMatchRatio { get; }
}