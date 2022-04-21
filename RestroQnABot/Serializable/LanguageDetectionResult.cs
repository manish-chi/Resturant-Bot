using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestroQnABot.Serializable
{
    public class LanguageDetectionResult
    {
        public LanguageResult[] LanguageResult { get; set; }
    }

    public class LanguageResult
    {
        public string language { get; set; }
        public float score { get; set; }
        public bool isTranslationSupported { get; set; }
        public bool isTransliterationSupported { get; set; }
        public Alternative[] alternatives { get; set; }
    }

    public class Alternative
    {
        public string language { get; set; }
        public float score { get; set; }
        public bool isTranslationSupported { get; set; }
        public bool isTransliterationSupported { get; set; }
    }


    public class DetectLanguageRequest
    {
        public TextInput[] TextInputs { get; set; }
    }

    public class TextInput
    {
        public string Text { get; set; }
    }

}
