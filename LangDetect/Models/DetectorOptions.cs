namespace LangDetect.Models;

/// <summary>
/// Configuration options for the language detection pipeline.
/// Pass an instance to <c>ILanguageDetectorFactory.Create()</c>
/// or configure via <c>AddLanguageDetector()</c> in DI setup.
/// </summary>
public record DetectorOptions
{
    /// <summary>
    /// Minimum confidence score required for a detection result to be
    /// considered reliable. Results below this value will have
    /// <c>IsReliable = false</c> on the <see cref="DetectionResult"/>.
    /// Valid range: 0.0 – 1.0.
    /// Default: 0.75
    /// </summary>
    public float ConfidenceThreshold { get; init; } = 0.75f;

    /// <summary>
    /// When true, the pipeline stops and returns immediately once any
    /// stage produces a result at or above <see cref="ConfidenceThreshold"/>.
    /// Disable if you want all stages to run regardless (useful for debugging).
    /// Default: true
    /// </summary>
    public bool EnableEarlyExit { get; init; } = true;

    /// <summary>
    /// Maximum number of tokens (words) the pipeline will process.
    /// Input exceeding this limit is silently truncated before analysis.
    /// Prevents unexpectedly slow detection on very large strings.
    /// Default: 500
    /// </summary>
    public int MaxTokens { get; init; } = 500;

    /// <summary>
    /// Minimum number of characters required to attempt detection.
    /// Inputs shorter than this return <see cref="DetectionResult"/> with
    /// <c>Language.Unknown</c> and <c>IsReliable = false</c> immediately.
    /// Default: 3
    /// </summary>
    public int MinInputLength { get; init; } = 3;


    /// <summary>
    /// Minimum ratio of non-Latin Unicode characters required to
    /// route input through the Unicode detection path.
    /// Inputs below this threshold skip straight to word frequency
    /// matching even if they contain some non-Latin characters.
    /// Default: 0.25 (at least 25% of characters must be non-Latin)
    /// </summary>
    public float MinNonLatinRatio { get; init; } = 0.25f;

    /// <summary>
    /// Controls which word list size is used by the detection pipeline.
    /// Default: Large (1000 words)
    /// </summary>
    public WordListSize WordListSize { get; init; } = WordListSize.Medium;

    /// <summary>
    /// Optional logger action for troubleshooting embedded resource
    /// loading and pipeline decisions. Set this to see diagnostic output.
    /// Example: options.Logger = Console.WriteLine;
    /// </summary>
    public Action<string>? Logger { get; init; } = null;

    /// <summary>
    /// Validates that all option values are within acceptable ranges.
    /// Called internally by <c>LanguageDetectorFactory</c> before building the pipeline.
    /// Throws <see cref="ArgumentOutOfRangeException"/> on invalid values.
    /// </summary>
    internal void Validate()
    {
        if (ConfidenceThreshold is < 0.0f or > 1.0f)
            throw new ArgumentOutOfRangeException(
                nameof(ConfidenceThreshold),
                "Must be between 0.0 and 1.0.");

        if (MaxTokens <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(MaxTokens),
                "Must be greater than zero.");

        if (MinInputLength <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(MinInputLength),
                "Must be greater than zero.");
    }
}