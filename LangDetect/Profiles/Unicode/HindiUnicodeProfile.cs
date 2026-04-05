using LangDetect.Models;

namespace LangDetect.Profiles.Unicode;

/// <summary>
/// Unicode profile for Hindi — Devanagari script (U+0900–U+097F).
/// Also covers other Devanagari-script languages (Nepali, Marathi)
/// but Hindi is the primary target for v1.
/// </summary>
public sealed class HindiUnicodeProfile : BaseUnicodeProfile
{
    public override Language Language => Language.Hindi;
    public override int RangeStart => 0x0900;
    public override int RangeEnd => 0x097F;
    public override float MinCoverage => 0.5f;
}