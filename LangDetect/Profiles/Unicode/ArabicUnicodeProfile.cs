using LangDetect.Models;

namespace LangDetect.Profiles.Unicode;

/// <summary>
/// Unicode profile for Arabic — Arabic script (U+0600–U+06FF).
/// Arabic is written right-to-left; the detection logic is direction-agnostic
/// since we only inspect code point values, not glyph order.
/// </summary>
public sealed class ArabicUnicodeProfile : BaseUnicodeProfile
{
    public override Language Language => Language.Arabic;
    public override int RangeStart => 0x0600;
    public override int RangeEnd => 0x06FF;
    public override float MinCoverage => 0.5f;
}