using LangDetect.Models;

namespace LangDetect.Profiles.Unicode;

/// <summary>
/// Unicode profile for Sinhala — Sinhala script (U+0D80–U+0DFF).
/// Sinhala script has no overlap with any other supported language.
/// When this profile does not match (romanized Singlish input),
/// detection falls through to <c>CommonWordDetectionStage</c>
/// which uses <c>SinglishWordList</c> — still returning
/// <see cref="Language.Sinhala"/> as the result.
/// </summary>
public sealed class SinhalaUnicodeProfile : BaseUnicodeProfile
{
    public override Language Language => Language.Sinhala;
    public override int RangeStart => 0x0D80;
    public override int RangeEnd => 0x0DFF;
    public override float MinCoverage => 0.5f;
}