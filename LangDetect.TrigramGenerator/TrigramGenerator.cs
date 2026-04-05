using System.Text.Json;

// point this at your wordlists folder
var wordlistsPath = args.Length > 0
    ? args[0]
    : Path.Combine("..", "LangDetect", "Resources", "Wordlists");

var outputPath = args.Length > 1
    ? args[1]
    : Path.Combine("..", "LangDetect", "Resources", "Trigrams");

Directory.CreateDirectory(outputPath);

// map of language name → wordlist filename prefix
var languages = new Dictionary<string, string>
{
    ["English"] = "English",
    ["Arabic"] = "Arabic",
    ["Hindi"] = "Hindi",
    ["Mandarin"] = "Mandarin",
    ["Japanese"] = "Japanese",
    ["Korean"] = "Korean",
    ["Sinhala"] = "Sinhala",
    ["Tamil"] = "Tamil",
};

var sizes = new[] { 200, 500, 1000 };

foreach (var (language, prefix) in languages)
{
    foreach (var size in sizes)
    {
        var wordlistFile = Path.Combine(wordlistsPath, $"{prefix}-{size}-Wordlist.txt");

        if (!File.Exists(wordlistFile))
        {
            Console.WriteLine($"SKIP: {wordlistFile} not found");
            continue;
        }

        var words = File.ReadAllLines(wordlistFile)
            .Select(l => l.Trim().ToLowerInvariant())
            .Where(l => l.Length > 0 && !l.StartsWith('#'))
            .ToList();

        var trigrams = GenerateTrigrams(words);
        var normalized = Normalize(trigrams);

        var outputFile = Path.Combine(outputPath, $"{prefix}-{size}-Trigrams.json");
        var json = JsonSerializer.Serialize(normalized, new JsonSerializerOptions
        {
            WriteIndented = true,
        });

        File.WriteAllText(outputFile, json);
        Console.WriteLine($"OK: {outputFile} ({normalized.Count} trigrams)");
    }
}

Console.WriteLine("Done.");

static Dictionary<string, int> GenerateTrigrams(List<string> words)
{
    var counts = new Dictionary<string, int>();

    foreach (var word in words)
    {
        if (word.Length < 1)
            continue;

        var padded = $" {word} ";

        for (int i = 0; i <= padded.Length - 3; i++)
        {
            var trigram = padded.Substring(i, 3);
            counts.TryGetValue(trigram, out var count);
            counts[trigram] = count + 1;
        }
    }

    return counts;
}

static Dictionary<string, float> Normalize(Dictionary<string, int> counts)
{
    if (counts.Count == 0)
        return [];

    var max = (float)counts.Values.Max();

    return counts
        .OrderByDescending(kvp => kvp.Value)
        .Take(300) // keep top 300 most frequent trigrams per language
        .ToDictionary(kvp => kvp.Key, kvp => kvp.Value / max);
}