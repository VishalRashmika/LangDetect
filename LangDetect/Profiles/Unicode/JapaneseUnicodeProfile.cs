using LangDetect.Models;

namespace LangDetect.Profiles.Unicode;

/// <summary>
/// Unicode profile for Japanese — covers both Hiragana (U+3040–U+309F)
/// and Katakana (U+30A0–U+30FF) as a combined range (U+3040–U+30FF).
/// Japanese text typically mixes Hiragana, Katakana, and Kanji —
/// the presence of Hiragana or Katakana is the reliable discriminator
/// that separates Japanese from Mandarin (which uses CJK only).
/// MinCoverage is lower because Japanese text is naturally mixed-script.
/// </summary>
public sealed class JapaneseUnicodeProfile : BaseUnicodeProfile
{
    public override Language Language => Language.Japanese;
    public override int RangeStart => 0x3040;
    public override int RangeEnd => 0x30FF;
    public override float MinCoverage => 0.1f;
}