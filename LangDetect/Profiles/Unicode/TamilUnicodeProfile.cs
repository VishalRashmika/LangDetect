using LangDetect.Models;

namespace LangDetect.Profiles.Unicode;

/// <summary>
/// Unicode profile for Tamil — Tamil script (U+0B80–U+0BFF).
/// Tamil script has no overlap with any other supported language.
/// When this profile does not match (romanized Tanglish input),
/// detection falls through to <c>CommonWordDetectionStage</c>
/// which uses <c>TanglishWordList</c> — still returning
/// <see cref="Language.Tamil"/> as the result.
/// </summary>
public sealed class TamilUnicodeProfile : BaseUnicodeProfile
{
    public override Language Language => Language.Tamil;
    public override int RangeStart => 0x0B80;
    public override int RangeEnd => 0x0BFF;
    public override float MinCoverage => 0.5f;
}