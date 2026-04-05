using LangDetect.Models;

namespace LangDetect.WordLists;

/// <summary>
/// Word list for romanized Tamil (Tanglish).
/// All entries are Latin script — romanized representations of
/// common Tamil words (e.g. "naan", "enna", "romba", "illai").
/// Returns <see cref="Language.Tamil"/> 
/// </summary>
public sealed class TamilWordList : BaseWordList
{
    public override Language Language => Language.Tamil;
    public override float MinMatchRatio => 0.04f;

    public TamilWordList(WordListSize size, Action<string>? logger = null) : base(size, logger) { }

    protected override string GetResourceName(WordListSize size) => size switch
    {
        WordListSize.Small => "LangDetect.Resources.Wordlists.Tamil-200-Wordlist.txt",
        WordListSize.Medium => "LangDetect.Resources.Wordlists.Tamil-500-Wordlist.txt",
        WordListSize.Large => "LangDetect.Resources.Wordlists.Tamil-1000-Wordlist.txt",
        _ => "LangDetect.Resources.Wordlists.Tamil-500-Wordlist.txt",
    };
}