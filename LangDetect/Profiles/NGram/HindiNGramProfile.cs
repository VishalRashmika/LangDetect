namespace LangDetect.Profiles.NGram;

public sealed class HindiNGramProfile : BaseNGramProfile
{
    public override Language Language => Language.Hindi;

    public HindiNGramProfile(WordListSize size, Action<string>? logger = null)
        : base(size, logger) { }

    protected override string GetResourceName(WordListSize size) => size switch
    {
        WordListSize.Small => "LangDetect.Resources.Trigrams.Hindi-200-Trigrams.json",
        WordListSize.Medium => "LangDetect.Resources.Trigrams.Hindi-500-Trigrams.json",
        WordListSize.Large => "LangDetect.Resources.Trigrams.Hindi-1000-Trigrams.json",
        _ => "LangDetect.Resources.Trigrams.Hindi-500-Trigrams.json",
    };
}