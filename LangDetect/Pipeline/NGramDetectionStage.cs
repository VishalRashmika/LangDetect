namespace LangDetect.Pipeline;

/// <summary>
/// Stage 3 of the detection pipeline.
/// Runs under the same conditions as <c>CommonWordDetectionStage</c> —
/// on the Latin path or as a fallback after the Unicode path.
/// Uses character trigram frequency profiles to boost accuracy for
/// short inputs where word-list matching yields too few token hits.
/// </summary>
public sealed class NGramDetectionStage : IDetectionStage
{
    private readonly IReadOnlyList<INGramProfile> _profiles;

    public int Priority => 3;

    public NGramDetectionStage(IReadOnlyList<INGramProfile> profiles)
    {
        _profiles = profiles;
    }

    /// <summary>
    /// Extracts trigrams from <see cref="DetectionContext.Tokens"/> via
    /// <see cref="TextUtility.ExtractTrigrams"/>, then scores each
    /// <see cref="INGramProfile"/> by summing the weights of matched trigrams.
    /// Normalizes scores via <see cref="ConfidenceAggregator.NormalizeScores"/>
    /// before selecting the winner.
    /// Returns <see cref="StageResult.Empty"/> when no trigrams match.
    /// </summary>
    public StageResult Analyze(DetectionContext ctx)
    {
        if (ctx.Tokens.Length == 0)
            return StageResult.Empty(nameof(NGramDetectionStage));

        var trigrams = TextUtility.ExtractTrigrams(ctx.Tokens).ToList();

        if (trigrams.Count == 0)
            return StageResult.Empty(nameof(NGramDetectionStage));

        var rawScores = new Dictionary<Language, float>();

        foreach (var profile in _profiles)
        {
            float score = 0f;
            foreach (var trigram in trigrams)
            {
                if (profile.Trigrams.TryGetValue(trigram, out var weight))
                    score += weight;
            }

            if (score > 0f)
                rawScores[profile.Language] = score;
        }

        if (rawScores.Count == 0)
            return StageResult.Empty(nameof(NGramDetectionStage));

        var normalized = ConfidenceAggregator.NormalizeScores(rawScores);
        var best = normalized.MaxBy(kvp => kvp.Value);

        return new StageResult
        {
            Language = best.Key,
            Confidence = best.Value,
            StageName = nameof(NGramDetectionStage),
        };
    }
}