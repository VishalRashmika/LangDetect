using LangDetect.Abstractions;
using LangDetect.Models;

namespace LangDetect.Profiles.Unicode;

/// <summary>
/// Base implementation of <see cref="IUnicodeProfile"/>.
/// Provides the shared <see cref="Matches"/> implementation —
/// subclasses only need to supply the range constants and language.
/// </summary>
public abstract class BaseUnicodeProfile : IUnicodeProfile
{
    public abstract Language Language { get; }
    public abstract int RangeStart { get; }
    public abstract int RangeEnd { get; }
    public abstract float MinCoverage { get; }

    /// <summary>
    /// Computes the ratio of characters in <paramref name="text"/> that
    /// fall within [<see cref="RangeStart"/>, <see cref="RangeEnd"/>].
    /// Returns true when that ratio meets or exceeds <see cref="MinCoverage"/>.
    /// Whitespace characters are excluded from the denominator so that
    /// heavily spaced text does not unfairly dilute the coverage score.
    /// </summary>
    public bool Matches(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return false;

        int total = 0;
        int inRange = 0;

        foreach (var c in text)
        {
            if (char.IsWhiteSpace(c))
                continue;

            total++;
            if (c >= RangeStart && c <= RangeEnd)
                inRange++;
        }

        if (total == 0)
            return false;

        float coverage = (float)inRange / total;
        return coverage >= MinCoverage;
    }

    /// <summary>
    /// Returns the raw coverage ratio without applying <see cref="MinCoverage"/>.
    /// Used by <c>UnicodeDetectionStage</c> to build a confidence score
    /// rather than a binary match decision.
    /// </summary>
    public float GetCoverage(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return 0f;

        int total = 0;
        int inRange = 0;

        foreach (var c in text)
        {
            if (char.IsWhiteSpace(c))
                continue;

            total++;
            if (c >= RangeStart && c <= RangeEnd)
                inRange++;
        }

        return total == 0 ? 0f : (float)inRange / total;
    }
}