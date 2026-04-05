using LangDetect.Models;

namespace LangDetect.WordLists;

/// <summary>
/// Word list for English — top ~500 most frequent English words.
/// All entries are Latin script lowercase.
/// Source: populate from a standard English word frequency corpus.
/// File: src/LangDetect/Resources/WordLists/english.txt
/// </summary>
public sealed class EnglishWordList : BaseWordList
{
    public override Language Language => Language.English;
    public override float MinMatchRatio => 0.05f;

    public EnglishWordList(WordListSize size, Action<string>? logger = null) : base(size, logger) { }

    protected override string GetResourceName(WordListSize size) => size switch
    {
        WordListSize.Small => "LangDetect.Resources.Wordlists.English-200-Wordlist.txt",
        WordListSize.Medium => "LangDetect.Resources.Wordlists.English-500-Wordlist.txt",
        WordListSize.Large => "LangDetect.Resources.Wordlists.English-1000-Wordlist.txt",
        _ => "LangDetect.Resources.Wordlists.English-500-Wordlist.txt",
    };
}