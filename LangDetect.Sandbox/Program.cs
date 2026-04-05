using System;
using System.Collections.Generic;
using System.Text;
using LangDetect;
using LangDetect.Models;

namespace LanguageDetection.Sandbox
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.WriteLine("=== Language Detection Sandbox ===\n");

            // Choose word list size: Small (200), Medium (500), or Large (1000)
            var options = new DetectorOptions
            {
                WordListSize = WordListSize.Large   // Best accuracy
                // WordListSize = WordListSize.Medium  // Default
                // WordListSize = WordListSize.Small   // Lightweight
            };

            var factory = new LanguageDetectorFactory();
            var detector = factory.Create(options);

            Console.WriteLine($"Detector created with WordListSize = {options.WordListSize}");
            Console.WriteLine("Supported languages: English, Sinhala, Arabic, Mandarin, Japanese, Korean, Tamil, Hindi\n");

            // Collection of test cases: (Expected Language, Input Text, Description)
            var testCases = new List<(string Expected, string Text, string Description)>
            {
                // ----- Native scripts -----
                ("English", "The quick brown fox jumps over the lazy dog", "Native English"),
                ("Arabic", "مرحبا كيف حالك اليوم في العالم", "Native Arabic"),
                ("Mandarin", "你好世界这是一个测试句子", "Native Mandarin"),
                ("Japanese", "こんにちは世界、これはテストです", "Native Japanese"),
                ("Korean", "안녕하세요 세계 이것은 테스트입니다", "Native Korean"),
                ("Sinhala", "මම ගෙදර යනවා ඔබ කොහොමද", "Native Sinhala"),
                ("Tamil", "வணக்கம் நான் வீட்டிற்கு செல்கிறேன்", "Native Tamil"),
                ("Hindi", "नमस्ते आप कैसे हैं", "Native Hindi"),

                // ----- Romanized / transliterated -----
                ("Arabic", "As-salamu alaykum", "Romanized Arabic"),
                ("Mandarin", "Ni hao shi jie zhe shi yi ge ce shi ju zi", "Pinyin (Mandarin)"),
                ("Japanese", "Konnichiwa sekai, kore wa tesuto desu", "Romaji (Japanese)"),
                ("Korean", "Annyeonghaseyo segye igeoseun teseuteuimnida", "Romanized Korean"),
                ("Tamil", "Vanakkam naan veetukku selgiren", "Romanized Tamil"),
                ("Sinhala", "Mama gedara yanawa oba kohomada", "Romanized Sinhala"),
                ("Hindi", "Aapka naam kya hai?", "Romanized Hindi"),

                // ----- Mixed scripts (Unicode + ASCII) -----
                ("English", "Hello 世界", "English + Chinese"),
                ("Mandarin", "我喜欢 C# programming", "Chinese + English"),
                ("Japanese", "私は Python が好きです", "Japanese + English"),
                ("Korean", "나는 JavaScript를 좋아해요", "Korean + English"),
                ("Arabic", "أنا أحب C++", "Arabic + English"),

                // ----- Short texts (edge cases) -----
                ("English", "Hi", "Short English"),
                ("Mandarin", "你好", "Short Mandarin"),
                ("Japanese", "こんにちは", "Short Japanese"),
                ("Korean", "안녕", "Short Korean"),
                ("Arabic", "مرحبا", "Short Arabic"),
                ("Sinhala", "ආයුබෝවන්", "Short Sinhala"),
                ("Tamil", "வணக்கம்", "Short Tamil"),
                ("Hindi", "नमस्ते", "Short Hindi"),

                // ----- Special characters and punctuation -----
                ("English", "Hello, world! How are you? 😊", "Emojis & punctuation"),
                ("English", "It's a wonderful day, isn't it?", "Apostrophes"),
                ("English", "Café naïve résumé", "Diacritics"),
                ("English", "The /ˈkwɪk/ brown fox", "IPA symbols"),
                ("Mandarin", "Nǐ hǎo shì jiè", "Pinyin with tone marks"),
                ("English", "She said, \"Hello!\"", "Quotes and punctuation"),

                // ----- URLs and emails -----
                ("English", "https://www.example.com", "URL"),
                ("English", "user@example.com", "Email address"),

                // ----- Embedded foreign words -----
                ("English", "The word 'こんにちは' means hello in Japanese", "English with Japanese embedded"),
                ("Mandarin", "这是English混合的句子", "Mandarin with English embedded"),
                ("Arabic", "هذا نص عربي with English words", "Arabic with English embedded"),
                ("Hindi", "यह अंग्रेजी with English words है", "Hindi with English embedded"),

                // ----- Nonsense / random input -----
                ("Unknown", "asdfghjkl qwertyuiop zxcvbnm", "Random keyboard smash (Latin)"),
                ("Arabic", "خمقث طخهه عسق", "Random Arabic characters"),
                ("Mandarin", "的一是不了人", "Random Chinese characters"),
                ("Unknown", "123456789", "Numbers only"),
                ("Unknown", "!@#$%^&*()", "Punctuation only"),
                ("Unknown", "     ", "Whitespace only"),
                ("Unknown", "", "Empty string"),

                // ----- Edge: repeated characters -----
                ("Unknown", new string('A', 100), "100× 'A' (repeated character)")
            };

            // Header
            Console.WriteLine($"{"Exp.",-8} {"Detected",-10} {"Conf.",-6} {"Rel.",-5} {"Description",-40}");
            Console.WriteLine(new string('-', 75));

            int passed = 0, total = 0;

            foreach (var (expected, text, description) in testCases)
            {
                total++;
                string displayText = text?.Length > 40 ? text[..37] + "..." : text ?? "<null>";
                string detected;
                float confidence;
                bool reliable;
                bool pass;

                try
                {
                    var result = detector.Detect(text);
                    detected = result.Language.ToString();
                    confidence = result.Confidence;
                    reliable = result.IsReliable;
                    pass = string.Equals(detected, expected, StringComparison.OrdinalIgnoreCase);
                    if (pass) passed++;
                }
                catch (Exception ex)
                {
                    detected = "ERROR";
                    confidence = 0;
                    reliable = false;
                    pass = false;
                    Console.WriteLine($"✗ {expected,-8} {detected,-10} {0,-6:F2} {"N",-5} {description,-40} | {ex.Message}");
                    continue;
                }

                string status = pass ? "✓" : "✗";
                string reliableFlag = reliable ? "Y" : "N";
                Console.WriteLine($"{status} {expected,-8} {detected,-10} {confidence,-6:F2} {reliableFlag,-5} {description,-40}");
            }

            // Summary
            Console.WriteLine(new string('-', 75));
            Console.WriteLine($"Total tests : {total}");
            Console.WriteLine($"Passed      : {passed}");
            Console.WriteLine($"Failed      : {total - passed}");
            Console.WriteLine($"Success rate: {(double)passed / total * 100:F2}%");

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
    }
}