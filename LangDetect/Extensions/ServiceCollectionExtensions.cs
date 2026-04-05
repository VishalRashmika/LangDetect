using LangDetect.Abstractions;
using LangDetect.Models;
using LangDetect.Profiles.Unicode;
using LangDetect.Utility;

namespace LangDetect.Extensions;

/// <summary>
/// Extension methods for registering LangDetect services
/// with the .NET dependency injection container.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers <see cref="ILanguageDetector"/> and all pipeline
    /// dependencies as singletons. The detector is thread-safe and
    /// intended to be shared across the application lifetime.
    ///
    /// Usage:
    /// <code>
    /// services.AddLanguageDetector();
    ///
    /// services.AddLanguageDetector(options => {
    ///     options.ConfidenceThreshold = 0.80f;
    ///     options.EnableEarlyExit     = true;
    /// });
    /// </code>
    /// </summary>
    public static IServiceCollection AddLanguageDetector(
        this IServiceCollection services,
        Action<DetectorOptions>? configure = null)
    {
        var options = new DetectorOptions();
        configure?.Invoke(options);
        options.Validate();

        services.AddSingleton(options);
        services.AddSingleton<TextPreprocessor>();

        services.AddSingleton<IReadOnlyList<BaseUnicodeProfile>>(_ =>
        [
            new HindiUnicodeProfile(),
            new ArabicUnicodeProfile(),
            new MandarinUnicodeProfile(),
            new JapaneseUnicodeProfile(),
            new KoreanUnicodeProfile(),
            new SinhalaUnicodeProfile(),
            new TamilUnicodeProfile(),
        ]);

        services.AddSingleton<IReadOnlyList<IWordList>>(sp =>
        {
            var opts = sp.GetRequiredService<DetectorOptions>();
            return
            [
                new EnglishWordList(opts.WordListSize),
                new SinhalaWordList(opts.WordListSize),
                new TamilWordList(opts.WordListSize),
                new ArabicWordList(opts.WordListSize),
                new JapaneseWordList(opts.WordListSize),
                new KoreanWordList(opts.WordListSize),
                new MandarinWordList(opts.WordListSize),
            ];
        });

        services.AddSingleton<IReadOnlyList<INGramProfile>>(sp =>
        {
            var opts = sp.GetRequiredService<DetectorOptions>();
            return
            [
                new EnglishNGramProfile  (opts.WordListSize, opts.Logger),
                new ArabicNGramProfile   (opts.WordListSize, opts.Logger),
                new HindiNGramProfile    (opts.WordListSize, opts.Logger),
                new MandarinNGramProfile (opts.WordListSize, opts.Logger),
                new JapaneseNGramProfile (opts.WordListSize, opts.Logger),
                new KoreanNGramProfile   (opts.WordListSize, opts.Logger),
                new SinhalaNGramProfile  (opts.WordListSize, opts.Logger),
                new TamilNGramProfile    (opts.WordListSize, opts.Logger),
    ];
        });

        services.AddSingleton<IReadOnlyList<IDetectionStage>>(sp =>
        [
            new UnicodeDetectionStage(
                sp.GetRequiredService<IReadOnlyList<BaseUnicodeProfile>>()),
            new CommonWordDetectionStage(
                sp.GetRequiredService<IReadOnlyList<IWordList>>()),
            new NGramDetectionStage(
                sp.GetRequiredService<IReadOnlyList<INGramProfile>>()),
        ]);

        services.AddSingleton<ILanguageDetector, LanguageDetector>();
        services.AddSingleton<ILanguageDetectorFactory, LanguageDetectorFactory>();

        return services;
    }
}