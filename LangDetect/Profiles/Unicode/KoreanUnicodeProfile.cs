using LangDetect.Models;

namespace LangDetect.Profiles.Unicode;

/// <summary>
/// Unicode profile for Korean — Hangul Syllables block (U+AC00–U+D7AF).
/// Hangul has no overlap with any other supported language script,
/// so MinCoverage can be kept relatively low while still being reliable.
/// </summary>
public sealed class KoreanUnicodeProfile : BaseUnicodeProfile
{
    public override Language Language => Language.Korean;
    public override int RangeStart => 0xAC00;
    public override int RangeEnd => 0xD7AF;
    public override float MinCoverage => 0.4f;
}