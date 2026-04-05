using LangDetect.Models;

namespace LangDetect.Abstractions;

/// <summary>
/// Defines a character trigram profile used by <c>NGramDetectionStage</c>.
/// Trigram profiles boost accuracy for short inputs where word-list
/// matching produces too few token matches to be reliable.
/// </summary>
public interface INGramProfile
{
    /// <summary>
    /// The language this profile identifies.
    /// </summary>
    Language Language { get; }

    /// <summary>
    /// A dictionary of character trigrams and their relative frequency
    /// weights for this language. Keys are lowercase 3-character strings.
    /// Values are normalized frequency scores (0.0 – 1.0).
    /// </summary>
    IReadOnlyDictionary<string, float> Trigrams { get; }
}