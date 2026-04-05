namespace LangDetect.Profiles.NGram;

public sealed class JapaneseNGramProfile : BaseNGramProfile
{
    public override Language Language => Language.Japanese;

    public JapaneseNGramProfile(WordListSize size, Action<string>? logger = null)
        : base(size, logger) { }

    protected override string GetResourceName(WordListSize size) => size switch
    {
        WordListSize.Small => "LangDetect.Resources.Trigrams.Japanese-200-Trigrams.json",
        WordListSize.Medium => "LangDetect.Resources.Trigrams.Japanese-500-Trigrams.json",
        WordListSize.Large => "LangDetect.Resources.Trigrams.Japanese-1000-Trigrams.json",
        _ => "LangDetect.Resources.Trigrams.Japanese-500-Trigrams.json",
    };
}