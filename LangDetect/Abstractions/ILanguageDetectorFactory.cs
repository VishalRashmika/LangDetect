using LangDetect.Models;

namespace LangDetect.Abstractions;

/// <summary>
/// Constructs a fully configured <see cref="ILanguageDetector"/> instance.
/// Use this directly when not using dependency injection.
/// </summary>
public interface ILanguageDetectorFactory
{
    /// <summary>
    /// Builds and returns a configured <see cref="ILanguageDetector"/>
    /// using the provided <paramref name="options"/>.
    /// Validates options before assembling the pipeline —
    /// throws <see cref="ArgumentOutOfRangeException"/> on invalid values.
    /// </summary>
    ILanguageDetector Create(DetectorOptions options);
}