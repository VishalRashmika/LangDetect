using LangDetect.Models;

namespace LangDetect.WordLists;

/// <summary>
/// Word list for romanized Sinhala (Singlish).
/// All entries are Latin script — romanized representations of
/// common Sinhala words (e.g. "mama", "amma", "koheda", "kiyala").
/// Returns <see cref="Language.Sinhala"/> — Singlish is not a
/// separate language, only a different script representation.
/// File: src/LangDetect/Resources/WordLists/singlish.txt
/// </summary>
public sealed class SinhalaWordList : BaseWordList
{
    public override Language Language => Language.Sinhala;
    public override float MinMatchRatio => 0.04f;

    public SinhalaWordList(WordListSize size, Action<string>? logger = null) : base(size, logger) { }

    protected override string GetResourceName(WordListSize size) => size switch
    {
        WordListSize.Small => "LangDetect.Resources.Wordlists.Sinhala-200-Wordlist.txt",
        WordListSize.Medium => "LangDetect.Resources.Wordlists.Sinhala-500-Wordlist.txt",
        WordListSize.Large => "LangDetect.Resources.Wordlists.Sinhala-1000-Wordlist.txt",
        _ => "LangDetect.Resources.Wordlists.Sinhala-500-Wordlist.txt",
    };
}