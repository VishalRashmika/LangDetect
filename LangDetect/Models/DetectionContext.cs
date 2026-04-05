namespace LangDetect.Models;

/// <summary>
/// Immutable snapshot of the input text after preprocessing.
/// Created once by <c>TextPreprocessor</c> and passed through
/// every stage in the pipeline — stages never modify it.
/// </summary>
public record DetectionContext
{
    /// <summary>
    /// The raw input string exactly as received from the caller.
    /// Never modified — preserved for diagnostic purposes.
    /// </summary>
    public required string OriginalText { get; init; }

    /// <summary>
    /// Lowercased, whitespace-normalized version of <see cref="OriginalText"/>.
    /// Used by all stages for consistent comparison.
    /// </summary>
    public required string NormalizedText { get; init; }

    /// <summary>
    /// Ratio of non-Latin Unicode characters to total non-whitespace
    /// characters. Computed once during preprocessing.
    /// Used by LanguageDetector alongside HasNonLatinUnicode to decide
    /// whether to route through the Unicode path.
    /// </summary>
    public required float NonLatinRatio { get; init; }

    /// <summary>
    /// Tokenized words from <see cref="NormalizedText"/>, truncated to
    /// <see cref="DetectorOptions.MaxTokens"/> before being stored.
    /// Used by <c>CommonWordDetectionStage</c> and <c>NGramDetectionStage</c>.
    /// </summary>
    public required string[] Tokens { get; init; }

    /// <summary>
    /// Total number of characters in <see cref="NormalizedText"/>.
    /// Used by <c>UnicodeDetectionStage</c> as the denominator
    /// when computing script coverage ratios.
    /// </summary>
    public required int CharCount { get; init; }

    /// <summary>
    /// True if <see cref="NormalizedText"/> contains at least one character
    /// outside Basic Latin (U+0000–U+007F).
    /// Computed once by <c>TextUtility.ContainsNonLatinUnicode</c> during
    /// preprocessing and used by <c>LanguageDetector</c> to route between
    /// the Unicode path and the Latin word-frequency path.
    /// </summary>
    public required bool HasNonLatinUnicode { get; init; }
}   