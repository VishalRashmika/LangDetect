using LangDetect.Models;

namespace LangDetect.Abstractions;

/// <summary>
/// Defines a Unicode script profile used by <c>UnicodeDetectionStage</c>
/// to identify a language by its character range coverage.
/// Implement one profile per supported non-Latin language.
/// </summary>
public interface IUnicodeProfile
{
    /// <summary>
    /// The language this profile identifies.
    /// </summary>
    Language Language { get; }

    /// <summary>
    /// Start of the Unicode code point range for this script (inclusive).
    /// </summary>
    int RangeStart { get; }

    /// <summary>
    /// End of the Unicode code point range for this script (inclusive).
    /// </summary>
    int RangeEnd { get; }

    /// <summary>
    /// Minimum ratio of characters that must fall within the range
    /// for a confident match. Range: 0.0 – 1.0.
    /// A lower value tolerates more mixed-script input.
    /// </summary>
    float MinCoverage { get; }

    /// <summary>
    /// Returns true if <paramref name="text"/> contains enough characters
    /// within <see cref="RangeStart"/>–<see cref="RangeEnd"/> to meet
    /// <see cref="MinCoverage"/>. Called by <c>UnicodeDetectionStage</c>
    /// for each registered profile.
    /// </summary>
    bool Matches(string text);
}