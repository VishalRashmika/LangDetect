using LangDetect.Models;

namespace LangDetect.Abstractions;

/// <summary>
/// Primary public interface for language detection.
/// Consume this interface in your application — never depend
/// directly on <c>LanguageDetector</c>.
/// </summary>
public interface ILanguageDetector
{
    /// <summary>
    /// Detects the language of the provided <paramref name="text"/>.
    /// Returns <see cref="DetectionResult.Unknown"/> when the input is
    /// too short, empty, or no stage produced a confident result.
    /// Never throws for valid string input — all errors surface as
    /// <see cref="DetectionResult.Unknown"/> with <c>IsReliable = false</c>.
    /// </summary>
    DetectionResult Detect(string text);
}