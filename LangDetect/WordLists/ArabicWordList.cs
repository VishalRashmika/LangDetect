using LangDetect.Models;

namespace LangDetect.WordLists;

/// <summary>
/// Word list for romanized Arabic.
/// All entries are Latin script — romanized representations of
/// common Arabic words (e.g. "kayfa", "antunna").
/// Returns <see cref="Language.Arabic"/>
/// </summary>
public sealed class ArabicWordList : BaseWordList
{
    public override Language Language => Language.Arabic;
    public override float MinMatchRatio => 0.06f;

    public ArabicWordList(WordListSize size, Action<string>? logger = null) : base(size, logger) { }

    protected override string GetResourceName(WordListSize size) => size switch
    {
        WordListSize.Small => "LangDetect.Resources.Wordlists.Arabic-200-Wordlist.txt",
        WordListSize.Medium => "LangDetect.Resources.Wordlists.Arabic-500-Wordlist.txt",
        WordListSize.Large => "LangDetect.Resources.Wordlists.Arabic-1000-Wordlist.txt",
        _ => "LangDetect.Resources.Wordlists.Arabic-500-Wordlist.txt",
    };
}