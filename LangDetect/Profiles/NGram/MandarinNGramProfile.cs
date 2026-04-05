namespace LangDetect.Profiles.NGram;

public sealed class MandarinNGramProfile : BaseNGramProfile
{
    public override Language Language => Language.Mandarin;

    public MandarinNGramProfile(WordListSize size, Action<string>? logger = null)
        : base(size, logger) { }

    protected override string GetResourceName(WordListSize size) => size switch
    {
        WordListSize.Small => "LangDetect.Resources.Trigrams.Mandarin-200-Trigrams.json",
        WordListSize.Medium => "LangDetect.Resources.Trigrams.Mandarin-500-Trigrams.json",
        WordListSize.Large => "LangDetect.Resources.Trigrams.Mandarin-1000-Trigrams.json",
        _ => "LangDetect.Resources.Trigrams.Mandarin-500-Trigrams.json",
    };
}