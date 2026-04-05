namespace LangDetect;

/// <summary>
/// Main orchestrator of the language detection pipeline.
/// Implements <see cref="ILanguageDetector"/> — consume via that
/// interface, never depend on this class directly.
///
/// Detection flow:
///   1. Preprocess input → DetectionContext (includes HasNonLatinUnicode gate)
///   2. If HasNonLatinUnicode → RunUnicodePath → early exit if confident
///   3. RunLatinPath (always runs if Unicode path did not exit early)
///   4. Merge results → build and return DetectionResult
/// </summary>
public class LanguageDetector : ILanguageDetector
{
    private readonly IReadOnlyList<IDetectionStage> _pipeline;
    private readonly TextPreprocessor _preprocessor;
    private readonly DetectorOptions _options;

    public LanguageDetector(
        IReadOnlyList<IDetectionStage> pipeline,
        TextPreprocessor preprocessor,
        DetectorOptions options)
    {
        _pipeline = pipeline.OrderBy(s => s.Priority).ToList();
        _preprocessor = preprocessor;
        _options = options;
    }

    /// <inheritdoc/>
    public DetectionResult Detect(string text)
    {
        if (text is null)
            return DetectionResult.Unknown;

        if (TextUtility.IsNullOrWhitespace(text))
            return DetectionResult.Unknown;

        var ctx = BuildContext(text);

        if (ctx.CharCount < _options.MinInputLength)
            return DetectionResult.Unknown;

        StageResult result;

        bool shouldRunUnicodePath = ctx.HasNonLatinUnicode && ctx.NonLatinRatio >= _options.MinNonLatinRatio;

        if (shouldRunUnicodePath)
        {
            result = RunUnicodePath(ctx);

            if (_options.EnableEarlyExit &&
                ConfidenceAggregator.ApplyThreshold(result, _options.ConfidenceThreshold))
                return BuildResult(result);

            // Unicode path ran but wasn't confident enough.
            // If non-Latin ratio is high (dominant script), trust the
            // Unicode result rather than falling through to Latin matching
            // which would incorrectly return English.
            if (ctx.NonLatinRatio >= 0.5f && result.HasResult)
                return BuildResult(result);
        }

        result = RunLatinPath(ctx);

        return BuildResult(result);
    }

    /// <summary>
    /// Builds a <see cref="DetectionContext"/> from raw input.
    /// Protected virtual — override in a subclass to customize
    /// preprocessing behaviour without replacing the full pipeline.
    /// </summary>
    protected virtual DetectionContext BuildContext(string text)
        => _preprocessor.Preprocess(text);

    /// <summary>
    /// Runs <c>UnicodeDetectionStage</c> (Priority = 1) only.
    /// Called exclusively when <see cref="DetectionContext.HasNonLatinUnicode"/>
    /// is true.
    /// </summary>
    private StageResult RunUnicodePath(DetectionContext ctx)
    {
        var unicodeStage = _pipeline.FirstOrDefault(s => s.Priority == 1);
        return unicodeStage?.Analyze(ctx)
            ?? StageResult.Empty(nameof(RunUnicodePath));
    }

    /// <summary>
    /// Runs all stages with Priority > 1 in order.
    /// Used for Latin-script input and as a fallback after an
    /// inconclusive Unicode path result.
    /// Applies early exit between stages when enabled.
    /// </summary>
    private StageResult RunLatinPath(DetectionContext ctx)
    {
        var results = new List<StageResult>();
        var latinStages = _pipeline.Where(s => s.Priority > 1);

        foreach (var stage in latinStages)
        {
            var result = stage.Analyze(ctx);
            results.Add(result);

            if (_options.EnableEarlyExit &&
                ConfidenceAggregator.ApplyThreshold(result, _options.ConfidenceThreshold))
                break;
        }

        return ConfidenceAggregator.MergeResults([.. results]);
    }

    /// <summary>
    /// Converts a <see cref="StageResult"/> into the final
    /// <see cref="DetectionResult"/> returned to the caller.
    /// </summary>
    private DetectionResult BuildResult(StageResult stageResult)
    {
        if (!stageResult.HasResult)
            return DetectionResult.Unknown;

        var language = stageResult.Language!.Value;

        return new DetectionResult
        {
            Language = language,
            Confidence = stageResult.Confidence,
            IsReliable = ConfidenceAggregator.ApplyThreshold(
                             stageResult, _options.ConfidenceThreshold),
            DetectedBy = stageResult.StageName,
            IsoCode = LanguageCode.ToIso(language),
        };
    }
}