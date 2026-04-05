namespace LangDetect.Profiles.NGram;

public sealed class KoreanNGramProfile : BaseNGramProfile
{
    public override Language Language => Language.Korean;

    public KoreanNGramProfile(WordListSize size, Action<string>? logger = null)
        : base(size, logger) { }

    protected override string GetResourceName(WordListSize size) => size switch
    {
        WordListSize.Small => "LangDetect.Resources.Trigrams.Korean-200-Trigrams.json",
        WordListSize.Medium => "LangDetect.Resources.Trigrams.Korean-500-Trigrams.json",
        WordListSize.Large => "LangDetect.Resources.Trigrams.Korean-1000-Trigrams.json",
        _ => "LangDetect.Resources.Trigrams.Korean-500-Trigrams.json",
    };
}