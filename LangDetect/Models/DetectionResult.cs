namespace LangDetect.Models;

/// <summary>
/// The final output returned to the caller by <c>ILanguageDetector.Detect()</c>.
/// Produced by <c>LanguageDetector</c> from the winning <see cref="StageResult"/>
/// after all pipeline stages have run or early exit has triggered.
/// </summary>
public record DetectionResult
{
    /// <summary>
    /// The detected language. Returns <see cref="Models.Language.Unknown"/>
    /// when no stage produced a result above the confidence threshold,
    /// or when the input was too short to analyze.
    /// </summary>
    public required Language Language { get; init; }

    /// <summary>
    /// Confidence score of the detection. Range: 0.0 – 1.0.
    /// 0.0 when <see cref="Language"/> is <see cref="Models.Language.Unknown"/>.
    /// </summary>
    public required float Confidence { get; init; }

    /// <summary>
    /// True when <see cref="Confidence"/> is at or above
    /// <see cref="DetectorOptions.ConfidenceThreshold"/>.
    /// Callers should check this before trusting the result.
    /// </summary>
    public required bool IsReliable { get; init; }

    /// <summary>
    /// Name of the pipeline stage that produced the winning result.
    /// Useful for diagnostics and understanding which detection
    /// technique fired. Empty string when Language is Unknown.
    /// </summary>
    public required string DetectedBy { get; init; }

    /// <summary>
    /// ISO 639-1 language code for the detected language (e.g. "en", "si", "ta").
    /// Returns "und" (ISO 639-3 undetermined) when Language is Unknown.
    /// Populated via <c>LanguageCode.ToIso()</c>.
    /// </summary>
    public required string IsoCode { get; init; }

    /// <summary>
    /// Returns a <see cref="DetectionResult"/> representing a failed detection.
    /// Used by <c>LanguageDetector</c> when input is too short, empty,
    /// or no stage cleared the confidence threshold.
    /// </summary>
    public static DetectionResult Unknown => new()
    {
        Language = Models.Language.Unknown,
        Confidence = 0f,
        IsReliable = false,
        DetectedBy = string.Empty,
        IsoCode = "und",
    };
}