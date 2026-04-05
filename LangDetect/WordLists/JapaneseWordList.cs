using LangDetect.Models;

namespace LangDetect.WordLists;

/// <summary>
/// Word list for romanized Japanese.
/// All entries are Latin script — romanized representations of
/// common Japanese words (e.g. "chotto", "sugoku").
/// Returns <see cref="Language.Japanese"/>
/// </summary>
public sealed class JapaneseWordList : BaseWordList
{
    public override Language Language => Language.Japanese;
    public override float MinMatchRatio => 0.07f;

    public JapaneseWordList(WordListSize size, Action<string>? logger = null) : base(size, logger) { }

    protected override string GetResourceName(WordListSize size) => size switch
    {
        WordListSize.Small => "LangDetect.Resources.Wordlists.Japanese-200-Wordlist.txt",
        WordListSize.Medium => "LangDetect.Resources.Wordlists.Japanese-500-Wordlist.txt",
        WordListSize.Large => "LangDetect.Resources.Wordlists.Japanese-1000-Wordlist.txt",
        _ => "LangDetect.Resources.Wordlists.Japanese-500-Wordlist.txt",
    };
}