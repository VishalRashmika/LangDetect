using System.Globalization;
using System.Text;

namespace LangDetect.Utility;

/// <summary>
/// Pure static text processing helpers shared across all pipeline stages.
/// No state, no dependencies — every method is independently unit testable.
/// </summary>
public static class TextUtility
{
    /// <summary>
    /// Returns true when <paramref name="text"/> is null, empty,
    /// or contains only whitespace characters.
    /// </summary>
    public static bool IsNullOrWhitespace(string? text)
        => string.IsNullOrWhiteSpace(text);

    /// <summary>
    /// Lowercases <paramref name="text"/> and collapses all consecutive
    /// whitespace sequences (spaces, tabs, newlines) into a single space.
    /// Trims leading and trailing whitespace.
    /// </summary>
    public static string Normalize(string text)
    {
        if (IsNullOrWhitespace(text))
            return string.Empty;

        // strip diacritics before any other processing so that
        // "café" becomes "cafe" and does not trigger the Unicode path
        text = StripDiacritics(text);

        var chars = text.ToLowerInvariant().AsSpan();
        var builder = new System.Text.StringBuilder(chars.Length);
        bool lastWasSpace = true;

        foreach (var c in chars)
        {
            if (char.IsWhiteSpace(c))
            {
                if (!lastWasSpace)
                {
                    builder.Append(' ');
                    lastWasSpace = true;
                }
            }
            else
            {
                builder.Append(c);
                lastWasSpace = false;
            }
        }

        return lastWasSpace && builder.Length > 0
            ? builder.ToString(0, builder.Length - 1)
            : builder.ToString();
    }

    /// <summary>
    /// Splits <paramref name="text"/> into word tokens on whitespace
    /// and punctuation boundaries. Returns an empty array for
    /// null or whitespace input. All tokens are already lowercase
    /// if input was produced by <see cref="Normalize"/>.
    /// </summary>
    public static string[] Tokenize(string text)
    {
        if (IsNullOrWhitespace(text))
            return [];

        // strip email domains and URL schemes before tokenizing
        // so "user@gmail.com" becomes "user" and
        // "https://example.com/path" becomes "example com path"
        text = StripEmailsAndUrls(text);

        return text
            .Split([' ', '\t', '\n', '\r', '.', ',', '!', '?',
                ':', ';', '"', '\'', '(', ')', '[', ']',
                '{', '}', '-', '_', '/', '\\', '@'],
                StringSplitOptions.RemoveEmptyEntries);
    }

    /// <summary>
    /// Returns true if <paramref name="text"/> contains at least one
    /// character outside the Basic Latin block (U+0000–U+007F).
    /// Used by <c>LanguageDetector</c> to route between the Unicode
    /// detection path and the Latin word-frequency path.
    /// This is the pipeline gate — called once per input during preprocessing.
    /// </summary>
    public static bool ContainsNonLatinUnicode(string text)
    {
        foreach (var c in text)
        {
            if (c > '\u007F')
                return true;
        }
        return false;
    }

    /// <summary>
    /// Counts how many characters in <paramref name="text"/> fall within
    /// the Unicode code point range [<paramref name="rangeStart"/>,
    /// <paramref name="rangeEnd"/>] inclusive.
    /// Used by <c>UnicodeDetectionStage</c> to compute script coverage ratio.
    /// </summary>
    public static int CountScriptChars(string text, int rangeStart, int rangeEnd)
    {
        if (IsNullOrWhitespace(text))
            return 0;

        int count = 0;
        foreach (var c in text)
        {
            if (c >= rangeStart && c <= rangeEnd)
                count++;
        }
        return count;
    }

    /// <summary>
    /// Extracts all character-level trigrams from <paramref name="tokens"/>.
    /// Each token is padded with a leading and trailing space so boundary
    /// trigrams are included. Returns an empty sequence for null or
    /// empty input. Used by <c>NGramDetectionStage</c>.
    /// Example: "cat" → " ca", "cat", "at "
    /// </summary>
    public static IEnumerable<string> ExtractTrigrams(string[] tokens)
    {
        if (tokens is null || tokens.Length == 0)
            yield break;

        foreach (var token in tokens)
        {
            if (token.Length < 1)
                continue;

            var padded = $" {token} ";
            for (int i = 0; i <= padded.Length - 3; i++)
                yield return padded.Substring(i, 3);
        }
    }

    /// <summary>
    /// Strips diacritic marks from Latin characters so that words like
    /// "café", "naïve", "résumé" are treated as plain ASCII Latin.
    /// Normalizes to Unicode Form D (decomposes characters into base +
    /// combining mark), then removes all combining characters.
    /// This prevents diacritic-heavy Latin text from incorrectly
    /// triggering the non-Latin Unicode detection path.
    /// </summary>
    private static string StripDiacritics(string text)
    {
        var normalized = text.Normalize(NormalizationForm.FormD);
        var builder = new System.Text.StringBuilder(normalized.Length);

        foreach (var c in normalized)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                builder.Append(c);
        }

        return builder.ToString().Normalize(NormalizationForm.FormC);
    }

    /// <summary>
    /// Returns the ratio of non-Latin Unicode characters to total
    /// non-whitespace characters in the text. Used by LanguageDetector
    /// to determine whether the input is genuinely non-Latin or only
    /// incidentally contains a few non-Latin characters.
    /// </summary>
    public static float GetNonLatinRatio(string text)
    {
        if (IsNullOrWhitespace(text))
            return 0f;

        int total = 0;
        int nonLatin = 0;

        foreach (var c in text)
        {
            if (char.IsWhiteSpace(c))
                continue;

            total++;
            if (c > '\u007F')
                nonLatin++;
        }

        return total == 0 ? 0f : (float)nonLatin / total;
    }


    /// <summary>
    /// Replaces email addresses and URLs with just their meaningful
    /// word components before tokenization.
    /// "user@gmail.com" → "user gmail com"
    /// "https://www.example.com/about" → "www example com about"
    /// </summary>
    private static string StripEmailsAndUrls(string text)
    {
        // remove common URL schemes
        text = text.Replace("https://", " ")
                   .Replace("http://", " ")
                   .Replace("www.", " ");

        // replace @ with space so email local part is kept as a token
        text = text.Replace('@', ' ');

        return text;
    }
}