using LangDetect.Models;

namespace LangDetect.WordLists;

/// <summary>
/// Word list for romanized Korean.
/// All entries are Latin script — romanized representations of
/// common Korean words (e.g. "hajiman", "bangbeop").
/// Returns <see cref="Language.Korean"/>
/// </summary>
public sealed class KoreanWordList : BaseWordList
{
    public override Language Language => Language.Korean;
    public override float MinMatchRatio => 0.07f;

    public KoreanWordList(WordListSize size, Action<string>? logger = null) : base(size, logger) { }

    protected override string GetResourceName(WordListSize size) => size switch
    {
        WordListSize.Small => "LangDetect.Resources.Wordlists.Korean-200-Wordlist.txt",
        WordListSize.Medium => "LangDetect.Resources.Wordlists.Korean-500-Wordlist.txt",
        WordListSize.Large => "LangDetect.Resources.Wordlists.Korean-1000-Wordlist.txt",
        _ => "LangDetect.Resources.Wordlists.Korean-500-Wordlist.txt",
    };
}