namespace LangDetect.Profiles.NGram;

/// <summary>
/// Base implementation of <see cref="INGramProfile"/>.
/// Loads trigram frequency data from an embedded JSON resource.
/// Subclasses supply the resource name based on the chosen
/// <see cref="WordListSize"/>.
/// </summary>
public abstract class BaseNGramProfile : INGramProfile
{
    public abstract Language Language { get; }

    protected abstract string GetResourceName(WordListSize size);

    private readonly Lazy<IReadOnlyDictionary<string, float>> _trigrams;
    private readonly Action<string>? _logger;

    public IReadOnlyDictionary<string, float> Trigrams => _trigrams.Value;

    protected BaseNGramProfile(WordListSize size, Action<string>? logger = null)
    {
        _logger = logger;
        _trigrams = new Lazy<IReadOnlyDictionary<string, float>>(
            () => LoadTrigrams(GetResourceName(size)));
    }

    private IReadOnlyDictionary<string, float> LoadTrigrams(string resourceName)
    {
        var assembly = typeof(BaseNGramProfile).Assembly;

        _logger?.Invoke($"[LangDetect] NGram loading: '{resourceName}'");

        using var stream = assembly.GetManifestResourceStream(resourceName);

        if (stream is null)
        {
            _logger?.Invoke($"[LangDetect] NGram FAILED: '{resourceName}' not found.");
            throw new InvalidOperationException(
                $"Embedded trigram resource '{resourceName}' not found. " +
                $"Run LangDetect.TrigramGenerator to generate trigram files " +
                $"and ensure they are set to EmbeddedResource.");
        }

        using var reader = new StreamReader(stream, System.Text.Encoding.UTF8);
        var json = reader.ReadToEnd();

        var result = System.Text.Json.JsonSerializer
            .Deserialize<Dictionary<string, float>>(json)
            ?? throw new InvalidOperationException(
                $"Failed to deserialize trigram data from '{resourceName}'.");

        _logger?.Invoke(
            $"[LangDetect] NGram SUCCESS: '{Language}' — {result.Count} trigrams loaded.");

        return result;
    }
}