using LangDetect.Abstractions;
using LangDetect.Models;

namespace LangDetect.WordLists;

/// <summary>
/// Base implementation of <see cref="IWordList"/>.
/// Loads word entries from an embedded .txt resource at construction time.
/// Each subclass points to its own resource file via <see cref="ResourceName"/>.
/// </summary>
//public abstract class BaseWordList : IWordList
//{
//    public abstract Language Language { get; }
//    public abstract float MinMatchRatio { get; }

//    /// <summary>
//    /// The embedded resource name for this word list file.
//    /// Convention: "LangDetect.Resources.WordLists.filename.txt"
//    /// </summary>
//    protected abstract string ResourceName { get; }

//    private readonly Lazy<IReadOnlySet<string>> _words;

//    public IReadOnlySet<string> Words => _words.Value;

//    protected BaseWordList()
//    {
//        _words = new Lazy<IReadOnlySet<string>>(LoadWords);
//    }

//    /// <summary>
//    /// Reads the embedded resource line by line, trims whitespace,
//    /// lowercases each entry, and discards blank lines or comment
//    /// lines beginning with '#'.
//    /// </summary>
//    private IReadOnlySet<string> LoadWords()
//    {
//        var assembly = typeof(BaseWordList).Assembly;
//        using var stream = assembly.GetManifestResourceStream(ResourceName)
//            ?? throw new InvalidOperationException(
//                $"Embedded resource '{ResourceName}' not found. " +
//                $"Ensure the file exists and its Build Action is set to Embedded Resource.");

//        using var reader = new StreamReader(stream, System.Text.Encoding.UTF8);
//        var words = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

//        string? line;
//        while ((line = reader.ReadLine()) is not null)
//        {
//            var trimmed = line.Trim();
//            if (trimmed.Length == 0 || trimmed.StartsWith('#'))
//                continue;

//            words.Add(trimmed.ToLowerInvariant());
//        }

//        return words;
//    }
//}


public abstract class BaseWordList : IWordList
{
    public abstract Language Language { get; }
    public abstract float MinMatchRatio { get; }

    protected abstract string GetResourceName(WordListSize size);

    private readonly Lazy<IReadOnlySet<string>> _words;
    private readonly Action<string>? _logger;

    public IReadOnlySet<string> Words => _words.Value;

    protected BaseWordList(WordListSize size, Action<string>? logger = null)
    {
        _logger = logger;
        _words = new Lazy<IReadOnlySet<string>>(
            () => LoadWords(GetResourceName(size)));
    }

    private IReadOnlySet<string> LoadWords(string resourceName)
    {
        var assembly = typeof(BaseWordList).Assembly;

        _logger?.Invoke($"[LangDetect] Attempting to load resource: '{resourceName}'");

        var availableResources = assembly.GetManifestResourceNames();
        _logger?.Invoke($"[LangDetect] Available resources ({availableResources.Length}):");
        foreach (var name in availableResources)
            _logger?.Invoke($"[LangDetect]   {name}");

        using var stream = assembly.GetManifestResourceStream(resourceName);

        if (stream is null)
        {
            var msg = $"[LangDetect] FAILED: Resource '{resourceName}' not found. " +
                      $"Check filename casing and Build Action = EmbeddedResource.";
            _logger?.Invoke(msg);
            throw new InvalidOperationException(msg);
        }

        _logger?.Invoke($"[LangDetect] SUCCESS: Loaded '{resourceName}'");

        using var reader = new StreamReader(stream, System.Text.Encoding.UTF8);
        var words = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        string? line;
        while ((line = reader.ReadLine()) is not null)
        {
            var trimmed = line.Trim();
            if (trimmed.Length == 0 || trimmed.StartsWith('#'))
                continue;

            words.Add(trimmed.ToLowerInvariant());
        }

        _logger?.Invoke($"[LangDetect] Loaded {words.Count} words for '{Language}'");
        return words;
    }
}