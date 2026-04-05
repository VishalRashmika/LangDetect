namespace LangDetect.Models;

/// <summary>
/// The output produced by a single <c>IDetectionStage</c>.
/// Passed to <c>ConfidenceAggregator</c> after each stage runs.
/// </summary>
public record StageResult
{
    /// <summary>
    /// The language identified by this stage.
    /// Null when the stage could not identify any language
    /// with meaningful confidence — use <see cref="Empty"/> in that case.
    /// </summary>
    public Language? Language { get; init; }

    /// <summary>
    /// Confidence score produced by this stage. Range: 0.0 – 1.0.
    /// A value of 0.0 indicates the stage found no signal.
    /// </summary>
    public float Confidence { get; init; }

    /// <summary>
    /// The name of the stage that produced this result.
    /// Used to populate <see cref="DetectionResult.DetectedBy"/>
    /// and for pipeline diagnostics.
    /// </summary>
    public required string StageName { get; init; }

    /// <summary>
    /// Convenience property. True when <see cref="Language"/> is not null
    /// and <see cref="Confidence"/> is greater than zero.
    /// </summary>
    public bool HasResult => Language is not null && Confidence > 0f;

    /// <summary>
    /// Returns a <see cref="StageResult"/> representing no finding.
    /// Stages should return this instead of null when they produce
    /// no usable signal for the given input.
    /// </summary>
    public static StageResult Empty(string stageName) => new()
    {
        Language = null,
        Confidence = 0f,
        StageName = stageName,
    };
}