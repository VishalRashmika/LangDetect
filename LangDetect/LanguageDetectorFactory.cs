using LangDetect.Abstractions;
using LangDetect.Models;
using LangDetect.Profiles.Unicode;
using LangDetect.Utility;
using LangDetect.WordLists;

namespace LangDetect;

/// <summary>
/// Wires all profiles, word lists, and stages into a configured
/// <see cref="LanguageDetector"/>. Use this directly when not
/// using dependency injection.
/// </summary>
public sealed class LanguageDetectorFactory : ILanguageDetectorFactory
{
    /// <inheritdoc/>
    public ILanguageDetector Create(DetectorOptions? options = null)
    {
        options ??= new DetectorOptions();
        options.Validate();

        var unicodeProfiles = BuildUnicodeProfiles();
        var wordLists = BuildWordLists(options);
        var ngramProfiles = BuildNGramProfiles(options);

        var pipeline = new List<IDetectionStage>
        {
            new UnicodeDetectionStage(unicodeProfiles),
            new CommonWordDetectionStage(wordLists),
            new NGramDetectionStage(ngramProfiles),
        };

        var preprocessor = new TextPreprocessor(options);

        return new LanguageDetector(pipeline, preprocessor, options);
    }

    private static IReadOnlyList<BaseUnicodeProfile> BuildUnicodeProfiles() =>
    [
        new HindiUnicodeProfile(),
        new ArabicUnicodeProfile(),
        new MandarinUnicodeProfile(),
        new JapaneseUnicodeProfile(),
        new KoreanUnicodeProfile(),
        new SinhalaUnicodeProfile(),
        new TamilUnicodeProfile(),
    ];

    private static IReadOnlyList<IWordList> BuildWordLists(DetectorOptions options) =>
    [
        new EnglishWordList(options.WordListSize, options.Logger),
        new SinhalaWordList(options.WordListSize, options.Logger),
        new TamilWordList(options.WordListSize, options.Logger),
        new ArabicWordList(options.WordListSize, options.Logger),
        new JapaneseWordList(options.WordListSize, options.Logger),
        new KoreanWordList(options.WordListSize, options.Logger),
        new MandarinWordList(options.WordListSize, options.Logger),
    ];

    /// <summary>
    /// NGram profiles are empty for v1 — the stage is present
    /// in the pipeline but will return StageResult.Empty until
    /// profiles are populated in v2.
    /// </summary>
    private static IReadOnlyList<INGramProfile> BuildNGramProfiles(DetectorOptions options) =>
[
    new EnglishNGramProfile  (options.WordListSize, options.Logger),
    new ArabicNGramProfile   (options.WordListSize, options.Logger),
    new HindiNGramProfile    (options.WordListSize, options.Logger),
    new MandarinNGramProfile (options.WordListSize, options.Logger),
    new JapaneseNGramProfile (options.WordListSize, options.Logger),
    new KoreanNGramProfile   (options.WordListSize, options.Logger),
    new SinhalaNGramProfile  (options.WordListSize, options.Logger),
    new TamilNGramProfile    (options.WordListSize, options.Logger),
];
}