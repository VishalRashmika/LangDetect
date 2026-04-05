namespace LangDetect.Profiles.NGram;

public sealed class ArabicNGramProfile : BaseNGramProfile
{
    public override Language Language => Language.Arabic;

    public ArabicNGramProfile(WordListSize size, Action<string>? logger = null)
        : base(size, logger) { }

    protected override string GetResourceName(WordListSize size) => size switch
    {
        WordListSize.Small => "LangDetect.Resources.Trigrams.Arabic-200-Trigrams.json",
        WordListSize.Medium => "LangDetect.Resources.Trigrams.Arabic-500-Trigrams.json",
        WordListSize.Large => "LangDetect.Resources.Trigrams.Arabic-1000-Trigrams.json",
        _ => "LangDetect.Resources.Trigrams.Arabic-500-Trigrams.json",
    };
}