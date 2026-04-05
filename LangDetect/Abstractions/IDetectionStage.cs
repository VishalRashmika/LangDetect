using LangDetect.Models;

namespace LangDetect.Abstractions;

/// <summary>
/// Represents a single stage in the language detection pipeline.
/// Stages are executed in ascending <see cref="Priority"/> order
/// by <c>LanguageDetector</c>. Each stage receives the same
/// <see cref="DetectionContext"/> and returns a <see cref="StageResult"/>.
/// </summary>
public interface IDetectionStage
{
    /// <summary>
    /// Execution order within the pipeline. Lower value runs first.
    /// Convention:
    ///   1 = UnicodeDetectionStage
    ///   2 = CommonWordDetectionStage
    ///   3 = NGramDetectionStage
    /// </summary>
    int Priority { get; }

    /// <summary>
    /// Analyzes the provided <paramref name="ctx"/> and returns a
    /// <see cref="StageResult"/> with the detected language and confidence.
    /// Must return <see cref="StageResult.Empty"/> when no signal is found —
    /// never return null.
    /// </summary>
    StageResult Analyze(DetectionContext ctx);
}