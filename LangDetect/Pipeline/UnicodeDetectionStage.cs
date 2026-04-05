namespace LangDetect.Pipeline;

/// <summary>
/// Stage 1 of the detection pipeline.
/// Only runs when <see cref="DetectionContext.HasNonLatinUnicode"/> is true.
/// Iterates all registered <see cref="IUnicodeProfile"/> implementations,
/// computes script coverage for each, and returns the highest-scoring match.
/// Produces high-confidence results for all non-Latin supported languages.
/// </summary>
public sealed class UnicodeDetectionStage : IDetectionStage
{
    private readonly IReadOnlyList<BaseUnicodeProfile> _profiles;

    public int Priority => 1;

    public UnicodeDetectionStage(IReadOnlyList<BaseUnicodeProfile> profiles)
    {
        _profiles = profiles;
    }

    /// <summary>
    /// Scores each profile using raw coverage ratio via
    /// <see cref="BaseUnicodeProfile.GetCoverage"/>, then returns
    /// the highest-scoring profile that also passes
    /// <see cref="IUnicodeProfile.Matches"/> as a hard gate.
    /// Returns <see cref="StageResult.Empty"/> when no profile matches.
    /// </summary>
    public StageResult Analyze(DetectionContext ctx)
    {
        if (string.IsNullOrWhiteSpace(ctx.NormalizedText))
            return StageResult.Empty(nameof(UnicodeDetectionStage));

        Language? bestLanguage = null;
        float bestConfidence = 0f;

        foreach (var profile in _profiles)
        {
            var coverage = profile.GetCoverage(ctx.NormalizedText);

            if (coverage >= profile.MinCoverage && coverage > bestConfidence)
            {
                bestLanguage = profile.Language;
                bestConfidence = coverage;
            }
        }

        if (bestLanguage is null)
            return StageResult.Empty(nameof(UnicodeDetectionStage));

        return new StageResult
        {
            Language = bestLanguage,
            Confidence = bestConfidence,
            StageName = nameof(UnicodeDetectionStage),
        };
    }
}