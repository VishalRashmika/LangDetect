namespace LangDetect.Profiles.NGram;

public sealed class EnglishNGramProfile : BaseNGramProfile
{
    public override Language Language => Language.English;

    public EnglishNGramProfile(WordListSize size, Action<string>? logger = null)
        : base(size, logger) { }

    protected override string GetResourceName(WordListSize size) => size switch
    {
        WordListSize.Small => "LangDetect.Resources.Trigrams.English-200-Trigrams.json",
        WordListSize.Medium => "LangDetect.Resources.Trigrams.English-500-Trigrams.json",
        WordListSize.Large => "LangDetect.Resources.Trigrams.English-1000-Trigrams.json",
        _ => "LangDetect.Resources.Trigrams.English-500-Trigrams.json",
    };
}