namespace LangDetect.Models;

/// <summary>
/// Controls which word list size is loaded by the detection pipeline.
/// Larger lists improve accuracy but increase memory usage slightly.
/// </summary>
public enum WordListSize
{
    /// <summary>
    /// 200-word lists — faster loading, lower memory, good for most use cases.
    /// </summary>
    Small = 200,

    /// <summary>
    /// 500-word lists — better accuracy for short or ambiguous inputs.
    /// </summary>
    Medium = 500,

    /// <summary>
    /// 1000-word lists — better in coverage.
    /// </summary>
    Large = 1000,

}