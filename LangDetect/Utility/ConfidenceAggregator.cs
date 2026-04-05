using LangDetect.Models;

namespace LangDetect.Utility;

/// <summary>
/// Pure static helpers for merging and evaluating stage confidence scores.
/// Called by <c>LanguageDetector</c> after each stage to decide
/// whether to exit early or continue the pipeline.
/// </summary>
public static class ConfidenceAggregator
{
    /// <summary>
    /// Normalizes a raw score dictionary so all values sum to 1.0.
    /// Returns an empty dictionary for null or all-zero input.
    /// Called internally by stages before returning a <see cref="StageResult"/>.
    /// </summary>
    public static Dictionary<Language, float> NormalizeScores(
        Dictionary<Language, float> scores)
    {
        if (scores is null || scores.Count == 0)
            return [];

        float total = 0f;
        foreach (var score in scores.Values)
            total += score;

        if (total == 0f)
            return [];

        var normalized = new Dictionary<Language, float>(scores.Count);
        foreach (var (lang, score) in scores)
            normalized[lang] = score / total;

        return normalized;
    }

    /// <summary>
    /// Returns true when <paramref name="result"/> has a language and its
    /// confidence meets or exceeds <paramref name="threshold"/>.
    /// Used by <c>LanguageDetector</c> to decide on early exit.
    /// </summary>
    public static bool ApplyThreshold(StageResult result, float threshold)
        => result.HasResult && result.Confidence >= threshold;

    /// <summary>
    /// Selects the highest-confidence result from <paramref name="results"/>.
    /// When two results share the same confidence the earlier one wins
    /// (preserves pipeline priority order).
    /// Returns <see cref="StageResult.Empty"/> when the array is empty
    /// or all results have no signal.
    /// </summary>
    public static StageResult MergeResults(StageResult[] results)
    {
        if (results is null || results.Length == 0)
            return StageResult.Empty("ConfidenceAggregator");

        StageResult? best = null;

        foreach (var result in results)
        {
            if (!result.HasResult)
                continue;

            if (best is null || result.Confidence > best.Confidence)
                best = result;
        }

        return best ?? StageResult.Empty("ConfidenceAggregator");
    }
}