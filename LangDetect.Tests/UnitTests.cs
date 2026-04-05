using System;
using LangDetect.Abstractions;
using LangDetect.Models;
using LangDetect;
using Xunit;

namespace LangDetect.Tests
{
    public class LanguageDetectionTests : IDisposable
    {
        private readonly ILanguageDetector _detector;

        public LanguageDetectionTests()
        {
            // Use Large wordlist for best accuracy (or change to Medium/Small as needed)
            var options = new DetectorOptions
            {
                WordListSize = WordListSize.Large
            };
            var factory = new LanguageDetectorFactory();
            _detector = factory.Create(options);
        }

        public void Dispose()
        {
            // Cleanup if needed
        }

        // --------------------------------------------------------------
        // Original language detection tests (native scripts)
        // --------------------------------------------------------------
        [Theory]
        [InlineData("English", "The quick brown fox jumps over the lazy dog")]
        [InlineData("Arabic", "مرحبا كيف حالك اليوم في العالم")]
        [InlineData("Mandarin", "你好世界这是一个测试句子")]
        [InlineData("Japanese", "こんにちは世界、これはテストです")]
        [InlineData("Korean", "안녕하세요 세계 이것은 테스트입니다")]
        [InlineData("Sinhala", "මම ගෙදර යනවා ඔබ කොහොමද")]
        [InlineData("Tamil", "வணக்கம் நான் வீட்டிற்கு செல்கிறேன்")]
        [InlineData("Hindi", "नमस्ते आप कैसे हैं")] // Hindi support added
        public void Detect_NativeScript_ReturnsCorrectLanguage(string expectedLanguage, string text)
        {
            var result = _detector.Detect(text);
            Assert.Equal(expectedLanguage, result.Language.ToString());
            Assert.True(result.Confidence > 0.5, $"Confidence too low: {result.Confidence}");
        }

        // --------------------------------------------------------------
        // Romanized forms (transliterated text)
        // --------------------------------------------------------------
        [Theory]
        [InlineData("Arabic", "As-salamu alaykum")]
        [InlineData("Mandarin", "Ni hao shi jie zhe shi yi ge ce shi ju zi")]
        [InlineData("Japanese", "Konnichiwa sekai, kore wa tesuto desu")]
        [InlineData("Korean", "Annyeonghaseyo segye igeoseun teseuteuimnida")]
        [InlineData("Tamil", "Vanakkam naan veetukku selgiren")]
        [InlineData("Sinhala", "Mama gedara yanawa oba kohomada")]
        [InlineData("Hindi", "Aapka naam kya hai?")]
        public void Detect_RomanizedForm_ReturnsCorrectLanguage(string expectedLanguage, string text)
        {
            var result = _detector.Detect(text);
            Assert.Equal(expectedLanguage, result.Language.ToString());
            // Romanized forms may have lower confidence; accept any > 0
            Assert.True(result.Confidence > 0, "Confidence should be > 0 for romanized text");
        }

        // --------------------------------------------------------------
        // Mixed scripts (Unicode + ASCII)
        // --------------------------------------------------------------
        [Theory]
        [InlineData("English", "Hello 世界")]
        [InlineData("Mandarin", "我喜欢 C# programming")]
        [InlineData("Japanese", "私は Python が好きです")]
        [InlineData("Korean", "나는 JavaScript를 좋아해요")]
        [InlineData("Arabic", "أنا أحب C++")]
        public void Detect_MixedScripts_ReturnsDominantLanguage(string expectedLanguage, string text)
        {
            var result = _detector.Detect(text);
            Assert.Equal(expectedLanguage, result.Language.ToString());
            // Mixed script confidence can be lower; just ensure it's not Unknown
            Assert.NotEqual("Unknown", result.Language.ToString());
        }

        // --------------------------------------------------------------
        // Non‑language inputs (should return Unknown)
        // --------------------------------------------------------------
        [Theory]
        [InlineData("123456789")]
        [InlineData("!@#$%^&*()")]
        [InlineData("   ")]
        [InlineData("")]
        [InlineData("a")]
        [InlineData("7")]
        [InlineData("asdfghjkl qwertyuiop zxcvbnm")] // random keyboard smash
        public void Detect_NonLanguageInput_ReturnsUnknown(string text)
        {
            var result = _detector.Detect(text);
            Assert.Equal("Unknown", result.Language.ToString());
            Assert.Equal(0, result.Confidence);
            Assert.False(result.IsReliable);
        }

        // --------------------------------------------------------------
        // Short texts (edge of detection)
        // --------------------------------------------------------------
        [Theory]
        [InlineData("English", "Hi")]
        [InlineData("Mandarin", "你好")]
        [InlineData("Japanese", "こんにちは")]
        [InlineData("Korean", "안녕")]
        [InlineData("Arabic", "مرحبا")]
        [InlineData("Sinhala", "ආයුබෝවන්")]
        [InlineData("Tamil", "வணக்கம்")]
        [InlineData("Hindi", "नमस्ते")]
        public void Detect_ShortText_ReturnsCorrectLanguage(string expectedLanguage, string text)
        {
            var result = _detector.Detect(text);
            Assert.Equal(expectedLanguage, result.Language.ToString());
            // Confidence may be low but should be > 0
            Assert.True(result.Confidence > 0, $"Confidence zero for short text '{text}'");
        }

        // --------------------------------------------------------------
        // Embedded foreign words (dominant language remains)
        // --------------------------------------------------------------
        [Theory]
        [InlineData("English", "The word 'こんにちは' means hello in Japanese")]
        [InlineData("Mandarin", "这是English混合的句子")]
        [InlineData("Arabic", "هذا نص عربي with English words")]
        [InlineData("Hindi", "यह अंग्रेजी with English words है")]
        public void Detect_EmbeddedForeignWords_ReturnsDominantLanguage(string expectedLanguage, string text)
        {
            var result = _detector.Detect(text);
            Assert.Equal(expectedLanguage, result.Language.ToString());
            Assert.True(result.Confidence > 0.3, $"Confidence too low: {result.Confidence}");
        }

        // --------------------------------------------------------------
        // Special characters and annotations
        // --------------------------------------------------------------
        [Theory]
        [InlineData("English", "The /ˈkwɪk/ brown fox")]
        [InlineData("Mandarin", "Nǐ hǎo shì jiè")] // Pinyin with tone marks
        [InlineData("English", "She said, \"Hello!\"")]
        [InlineData("English", "Hello, world! How are you? 😊")]
        [InlineData("English", "It's a wonderful day, isn't it?")]
        [InlineData("English", "Café naïve résumé")] // diacritics
        public void Detect_SpecialCharacters_ReturnsCorrectLanguage(string expectedLanguage, string text)
        {
            var result = _detector.Detect(text);
            Assert.Equal(expectedLanguage, result.Language.ToString());
        }

        // --------------------------------------------------------------
        // Repeated characters (should be Unknown or English with low confidence)
        // --------------------------------------------------------------
        [Theory]
        [InlineData(1000)]
        [InlineData(10000)]
        public void Detect_LongRepeatedCharacter_ReturnsUnknownOrLowConfidenceEnglish(int length)
        {
            string text = new string('A', length);
            var result = _detector.Detect(text);
            // Accept either Unknown or English with confidence < 0.6
            bool acceptable = result.Language.ToString() == "Unknown" ||
                              (result.Language.ToString() == "English" && result.Confidence < 0.6);
            Assert.True(acceptable, $"Detected {result.Language} with confidence {result.Confidence}");
        }

        // --------------------------------------------------------------
        // Null input (should not throw, return Unknown)
        // --------------------------------------------------------------
        [Fact]
        public void Detect_NullInput_ReturnsUnknownWithoutException()
        {
            var exception = Record.Exception(() => _detector.Detect(null));
            Assert.Null(exception);
            var result = _detector.Detect(null);
            Assert.Equal("Unknown", result.Language.ToString());
            Assert.Equal(0, result.Confidence);
        }

        // --------------------------------------------------------------
        // Random non‑Latin gibberish should map to appropriate language
        // --------------------------------------------------------------
        [Theory]
        [InlineData("Arabic", "خمقث طخهه عسق")]
        [InlineData("Mandarin", "的一是不了人")]
        public void Detect_RandomNonLatinGibberish_ReturnsExpectedLanguage(string expectedLanguage, string text)
        {
            var result = _detector.Detect(text);
            Assert.Equal(expectedLanguage, result.Language.ToString());
        }
    }
}