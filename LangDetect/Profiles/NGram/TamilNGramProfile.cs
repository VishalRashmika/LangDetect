namespace LangDetect.Profiles.NGram;

public sealed class TamilNGramProfile : BaseNGramProfile
{
    public override Language Language => Language.Tamil;

    public TamilNGramProfile(WordListSize size, Action<string>? logger = null)
        : base(size, logger) { }

    protected override string GetResourceName(WordListSize size) => size switch
    {
        WordListSize.Small => "LangDetect.Resources.Trigrams.Tamil-200-Trigrams.json",
        WordListSize.Medium => "LangDetect.Resources.Trigrams.Tamil-500-Trigrams.json",
        WordListSize.Large => "LangDetect.Resources.Trigrams.Tamil-1000-Trigrams.json",
        _ => "LangDetect.Resources.Trigrams.Tamil-500-Trigrams.json",
    };
}