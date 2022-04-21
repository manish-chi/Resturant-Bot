using RestroQnABot.Serializable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestroQnABot.Interfaces
{
    public interface ITranslationClient
    {
        Task<string> TranslateTextRequestAsync(string utterance, string languageCode);
        Task<TranslationResult[]> TranslateAdaptiveCardAsync(string requestBody, string languageCode);
        Task<string> DetectLanguageFromTextAsync(string utterance);
        
    }
}
