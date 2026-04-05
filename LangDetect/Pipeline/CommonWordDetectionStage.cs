namespace LangDetect.Pipeline;

/// <summary>
/// Stage 2 of the detection pipeline.
/// Runs on the Latin path (no non-Latin Unicode found) and also
/// as a fallback when <c>UnicodeDetectionStage</c> did not produce
/// a confident result.
/// Computes the ratio of input tokens that match each word list
/// and returns the language with the highest match ratio.
/// All word lists contain romanized Latin script only.
/// </summary>
public sealed class CommonWordDetectionStage : IDetectionStage
{
    private readonly IReadOnlyList<IWordList> _wordLists;

    public int Priority => 2;

    public CommonWordDetectionStage(IReadOnlyList<IWordList> wordLists)
    {
        _wordLists = wordLists;
    }

    /// <summary>
    /// For each word list, counts how many tokens appear in
    /// <see cref="IWordList.Words"/> and divides by total token count
    /// to produce a match ratio. Normalizes all ratios across languages
    /// via <see cref="ConfidenceAggregator.NormalizeScores"/> before
    /// selecting the winner.
    /// Returns <see cref="StageResult.Empty"/> when no tokens match
    /// any word list above <see cref="IWordList.MinMatchRatio"/>.
    /// </summary>
    public StageResult Analyze(DetectionContext ctx)
    {
        if (ctx.Tokens.Length == 0)
            return StageResult.Empty(nameof(CommonWordDetectionStage));

        var rawScores = new Dictionary<Language, float>();

        foreach (var wordList in _wordLists)
        {
            int matchCount = 0;
            foreach (var token in ctx.Tokens)
            {
                if (wordList.Words.Contains(token))
                    matchCount++;
            }

            float ratio = (float)matchCount / ctx.Tokens.Length;

            if (ratio >= wordList.MinMatchRatio)
                rawScores[wordList.Language] = ratio;
        }

        if (rawScores.Count == 0)
            return StageResult.Empty(nameof(CommonWordDetectionStage));

        var normalized = ConfidenceAggregator.NormalizeScores(rawScores);

        var best = normalized.MaxBy(kvp => kvp.Value);

        return new StageResult
        {
            Language = best.Key,
            Confidence = best.Value,
            StageName = nameof(CommonWordDetectionStage),
        };
    }
}