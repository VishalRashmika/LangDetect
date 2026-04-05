namespace LangDetect.Profiles.NGram;

public sealed class SinhalaNGramProfile : BaseNGramProfile
{
    public override Language Language => Language.Sinhala;

    public SinhalaNGramProfile(WordListSize size, Action<string>? logger = null)
        : base(size, logger) { }

    protected override string GetResourceName(WordListSize size) => size switch
    {
        WordListSize.Small => "LangDetect.Resources.Trigrams.Sinhala-200-Trigrams.json",
        WordListSize.Medium => "LangDetect.Resources.Trigrams.Sinhala-500-Trigrams.json",
        WordListSize.Large => "LangDetect.Resources.Trigrams.Sinhala-1000-Trigrams.json",
        _ => "LangDetect.Resources.Trigrams.Sinhala-500-Trigrams.json",
    };
}