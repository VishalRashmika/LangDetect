using LangDetect.Models;

namespace LangDetect.Utility;

/// <summary>
/// Converts raw input text into a <see cref="DetectionContext"/>
/// ready for pipeline consumption. Called once per <c>Detect()</c>
/// invocation — all stages receive the same context object.
/// </summary>
public class TextPreprocessor
{
    private readonly DetectorOptions _options;

    public TextPreprocessor(DetectorOptions options)
    {
        _options = options;
    }

    /// <summary>
    /// Normalizes, tokenizes, and inspects <paramref name="text"/>,
    /// returning a fully populated <see cref="DetectionContext"/>.
    /// Truncates tokens to <see cref="DetectorOptions.MaxTokens"/>.
    /// </summary>
    public DetectionContext Preprocess(string text)
    {
        var normalized = TextUtility.Normalize(text);

        var allTokens = TextUtility.Tokenize(normalized);
        var tokens = allTokens.Length > _options.MaxTokens
            ? allTokens[.._options.MaxTokens]
            : allTokens;

        return new DetectionContext
        {
            OriginalText = text,
            NormalizedText = normalized,
            Tokens = tokens,
            CharCount = normalized.Length,
            HasNonLatinUnicode = TextUtility.ContainsNonLatinUnicode(normalized),
            NonLatinRatio = TextUtility.GetNonLatinRatio(normalized),  // new
        };
    }

    /// <summary>
    /// Returns true when the character should be excluded from analysis.
    /// Override in a subclass to customize filtering behaviour —
    /// for example to preserve certain punctuation for a specific script.
    /// </summary>
    protected virtual bool ShouldSkipChar(char c)
        => char.IsControl(c) || char.IsSurrogate(c);
}