using LangDetect.Models;

namespace LangDetect.Profiles.Unicode;

/// <summary>
/// Unicode profile for Mandarin Chinese — CJK Unified Ideographs (U+4E00–U+9FFF).
/// Note: this range also covers Japanese Kanji and some Korean Hanja.
/// Disambiguation between Mandarin and Japanese is handled by
/// <c>JapaneseUnicodeProfile</c> taking precedence when Hiragana
/// or Katakana characters are also present.
/// </summary>
public sealed class MandarinUnicodeProfile : BaseUnicodeProfile
{
    public override Language Language => Language.Mandarin;
    public override int RangeStart => 0x4E00;
    public override int RangeEnd => 0x9FFF;
    public override float MinCoverage => 0.4f;
}