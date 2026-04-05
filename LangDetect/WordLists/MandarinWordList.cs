using LangDetect.Models;

namespace LangDetect.WordLists;

/// <summary>
/// Word list for romanized Mandarin.
/// All entries are Latin script — romanized representations of
/// common Mandarin words (e.g. "nian", "xuesheng").
/// Returns <see cref="Language.Mandarin"/>
/// </summary>
public sealed class MandarinWordList : BaseWordList
{
    public override Language Language => Language.Mandarin;
    public override float MinMatchRatio => 0.08f;

    public MandarinWordList(WordListSize size, Action<string>? logger = null) : base(size, logger) { }

    protected override string GetResourceName(WordListSize size) => size switch
    {
        WordListSize.Small => "LangDetect.Resources.Wordlists.Mandarin-200-Wordlist.txt",
        WordListSize.Medium => "LangDetect.Resources.Wordlists.Mandarin-500-Wordlist.txt",
        WordListSize.Large => "LangDetect.Resources.Wordlists.Mandarin-1000-Wordlist.txt",
        _ => "LangDetect.Resources.Wordlists.Mandarin-500-Wordlist.txt",
    };
}