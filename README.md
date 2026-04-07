# LangDetect.Net

> Lightweight, self-contained language detection for .NET — no cloud, no Python, no runtime dependencies.

[![NuGet](https://img.shields.io/nuget/v/LangDetect.Net.svg)](https://www.nuget.org/packages/LangDetect.Net)
[![NuGet Downloads](https://img.shields.io/nuget/dt/LangDetect.Net.svg)](https://www.nuget.org/packages/LangDetect.Net)
[![License: GPL-3.0](https://img.shields.io/badge/License-GPL--3.0-blue.svg)](https://opensource.org/licenses/GPL-3.0)
[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4)](https://dotnet.microsoft.com)

LangDetect is a .NET 8 class library for detecting the language of a given text string. It uses a multi-stage detection pipeline — Unicode script analysis, common word frequency matching, and character trigram profiling — to identify languages accurately across both native scripts and romanized Latin representations.

---

## Supported Languages

| Language | Native Script | Unicode Range | Romanized Detection |
|---|---|---|---|
| English | Latin | U+0000–U+007F | Native (Latin) |
| Arabic | Arabic | U+0600–U+06FF | Word list fallback |
| Hindi | Devanagari | U+0900–U+097F | Word list fallback |
| Mandarin | CJK Ideographs | U+4E00–U+9FFF | Word list fallback (Pinyin) |
| Japanese | Hiragana + Katakana | U+3040–U+30FF | Word list fallback (Romaji) |
| Korean | Hangul Syllables | U+AC00–U+D7AF | Word list fallback |
| Sinhala | Sinhala | U+0D80–U+0DFF | Word list fallback (Singlish) |
| Tamil | Tamil | U+0B80–U+0BFF | Word list fallback (Tanglish) |

---

## Installation

```bash
dotnet add package LangDetect.Net
```

Or via the NuGet Package Manager in Visual Studio — search for `LangDetect.Net`.

---

## Quick Start

```csharp
using LangDetect;
using LangDetect.Models;

// create a detector with default options
var factory  = new LanguageDetectorFactory();
var detector = factory.Create();

var result = detector.Detect("The quick brown fox jumps over the lazy dog");

Console.WriteLine(result.Language);    // English
Console.WriteLine(result.IsoCode);     // en
Console.WriteLine(result.Confidence);  // 1.00
Console.WriteLine(result.IsReliable);  // True
Console.WriteLine(result.DetectedBy);  // CommonWordDetectionStage
```

---

## Detection Result

Every call to `Detect()` returns a `DetectionResult` record:

```csharp
public record DetectionResult
{
    public Language Language     { get; init; }  // detected language or Unknown
    public float    Confidence   { get; init; }  // 0.0 – 1.0
    public bool     IsReliable   { get; init; }  // confidence >= configured threshold
    public string   DetectedBy   { get; init; }  // which pipeline stage fired
    public string   IsoCode      { get; init; }  // ISO 639-1 code e.g. "en", "si"
}
```

When detection fails or input is too short, `DetectionResult.Unknown` is returned — `Detect()` never throws for valid string input.

---

## Configuration

```csharp
var detector = new LanguageDetectorFactory().Create(new DetectorOptions
{
    ConfidenceThreshold = 0.80f,          // minimum score to be considered reliable
    EnableEarlyExit     = true,           // stop pipeline once confident result found
    WordListSize        = WordListSize.Large, // Small (200) | Medium (500) | Large (1000)
    MinInputLength      = 3,              // inputs shorter than this return Unknown
    MaxTokens           = 500,            // truncate long inputs before analysis
    MinNonLatinRatio    = 0.25f,          // minimum non-Latin ratio to trigger Unicode path
    Logger              = Console.WriteLine, // optional diagnostic logger
});
```

### Word list sizes

| Size | Words | Use case |
|---|---|---|
| `WordListSize.Small` | 200 | Memory-constrained environments, fast startup |
| `WordListSize.Medium` | 500 | Balanced — recommended default |
| `WordListSize.Large` | 1000 | Best accuracy, especially for short inputs |

---

## Detection Pipeline

LangDetect uses a three-stage pipeline. Each stage runs in priority order and the result is returned as soon as a confident detection is made (early exit).

```
Input text
    │
    ▼
TextPreprocessor
    │  normalize → tokenize → compute HasNonLatinUnicode + NonLatinRatio
    ▼
Does text contain non-Latin Unicode above MinNonLatinRatio threshold?
    │
    ├── YES → Stage 1: UnicodeDetectionStage
    │              Checks script coverage against 7 Unicode range profiles
    │              Confident result → return early
    │              Not confident   → fall through to Stage 2 + 3
    │
    └── NO  → Stage 2: CommonWordDetectionStage
                   Matches tokens against romanized word lists
                   Confident result → return early
                   Not confident   → Stage 3: NGramDetectionStage
                                         Scores character trigram profiles
                                         Returns best match or Unknown
```

### Stage details

| Stage | Priority | Technique | Best for |
|---|---|---|---|
| `UnicodeDetectionStage` | 1 | Script range coverage ratio | Arabic, Hindi, Mandarin, Japanese, Korean, Sinhala, Tamil in native script |
| `CommonWordDetectionStage` | 2 | Token frequency matching | English, romanized scripts |
| `NGramDetectionStage` | 3 | Character trigram scoring | Short inputs, ambiguous text |

---

## Examples

### Native script detection

```csharp
detector.Detect("مرحبا كيف حالك اليوم");
// → { Language: Arabic, Confidence: 1.00, IsReliable: true, IsoCode: "ar" }

detector.Detect("नमस्ते आप कैसे हैं");
// → { Language: Hindi, Confidence: 1.00, IsReliable: true, IsoCode: "hi" }

detector.Detect("こんにちは世界");
// → { Language: Japanese, Confidence: 0.81, IsReliable: true, IsoCode: "ja" }
```

### Romanized script detection

```csharp
detector.Detect("mama giye koheda kiyala amma");
// → { Language: Sinhala, Confidence: 0.85, IsReliable: true, IsoCode: "si" }

detector.Detect("naan pogiren enna romba thanks");
// → { Language: Tamil, Confidence: 0.80, IsReliable: true, IsoCode: "ta" }
```

### Graceful unknown handling

```csharp
detector.Detect("");          // → DetectionResult.Unknown
detector.Detect("123456");   // → DetectionResult.Unknown
detector.Detect(null);       // → DetectionResult.Unknown (never throws)
```

### Diagnostic logging

```csharp
var detector = new LanguageDetectorFactory().Create(new DetectorOptions
{
    Logger = msg => Debug.WriteLine(msg),
});
```

Output:
```
[LangDetect] Attempting to load resource: 'LangDetect.Resources.Wordlists.English-1000-Wordlist.txt'
[LangDetect] SUCCESS: Loaded 'LangDetect.Resources.Wordlists.English-1000-Wordlist.txt'
[LangDetect] Loaded 1000 words for 'English'
```

---

## ISO Language Codes

```csharp
using LangDetect.Utility;

LanguageCode.ToIso(Language.English);   // "en"
LanguageCode.ToIso(Language.Sinhala);   // "si"
LanguageCode.ToIso(Language.Unknown);   // "und"

LanguageCode.FromIso("ar");             // Language.Arabic
LanguageCode.FromIso("zh");             // Language.Mandarin
LanguageCode.FromIso("xyz");            // Language.Unknown
```

---

## Known Limitations

- **Mixed-script text**
- **Very short inputs**
- **Romanized Mandarin (Pinyin)**
- **No multi-language detection** — a single `Detect()` call returns one language. Mixed documents are planned for v2.
- **N-gram profiles are derived from word lists** — trigram quality is directly proportional to word list quality and size.

---

## Roadmap
### Planned features in V2
- Multi-language detection — ranked candidate list with per-language confidence scores
- Code-switching support — detect language changes within a single document
- Expanded language support — French, Spanish, Portuguese, German, Russian
- Compact ONNX model for Latin-script disambiguation
- Dialect identification — Mandarin vs Cantonese, Indian English vs British English
- Streaming / span detection over long documents
- Calibrated confidence scores via isotonic regression
- Proper benchmark suite with labeled test dataset

---

## Contributing

Contributions are welcome. Please open an issue before submitting a pull request for non-trivial changes.

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/my-feature`)
3. Commit your changes
4. Push to the branch and open a Pull Request

### Generating trigram data
If you update the word lists, regenerate the trigram JSON files using the included tool:

```bash
cd LangDetect.TrigramGenerator
dotnet run
```

Then copy the output from `Resources/Trigrams/` into `LangDetect/Resources/Trigrams/` and rebuild.

---

## License

This project is licensed under the [GNU General Public License v3.0](LICENSE).