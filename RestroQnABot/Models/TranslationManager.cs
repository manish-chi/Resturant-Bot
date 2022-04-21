using AdaptiveCards;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestroQnABot.Interfaces;
using RestroQnABot.Utlities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestroQnABot.Models
{
    public class TranslationManager
    {
        private ITranslationClient _translationClient;
       
        public TranslationManager(ITranslationClient translationClient)
        {
            _translationClient = translationClient;
        }

        public async Task<string> GetTranslatedTextAsync(string utterance, string languageCode)
        {

            return await _translationClient.TranslateTextRequestAsync(utterance, languageCode);
        }

        public async Task<string> DetectLanguageAsync(string utterance)
        {
            return await _translationClient.DetectLanguageFromTextAsync(utterance);
        }

        public async Task<object> TranslateAdaptiveCard(object card, string userLanguage)
        {

            var propertiesToTranslate = new[] { "text", "altText", "fallbackText", "title", "placeholder", "data" };

            var cardJObject = JObject.FromObject(card);
            var list = new List<(JContainer Container, object Key, string Text)>();

            void recurseThroughJObject(JObject jObject)
            {
                var type = jObject["type"];
                var parent = jObject.Parent;
                var grandParent = parent?.Parent;
                // value should be translated in facts and Input.Text, and ignored in Input.Date and Input.Time and Input.Toggle and Input.ChoiceSet and Input.Choice
                var valueIsTranslatable = type?.Type == JTokenType.String && (string)type == "Input.Text"
                    || type == null && parent?.Type == JTokenType.Array && grandParent?.Type == JTokenType.Property && ((JProperty)grandParent)?.Name == "facts";

                foreach (var key in ((IDictionary<string, JToken>)jObject).Keys)
                {
                    switchOnJToken(jObject, key, propertiesToTranslate.Contains(key) || (key == "value" && valueIsTranslatable));
                }
            }

            void switchOnJToken(JContainer jContainer, object key, bool shouldTranslate)
            {
                var jToken = jContainer[key];

                switch (jToken.Type)
                {
                    case JTokenType.Object:

                        recurseThroughJObject((JObject)jToken);
                        break;

                    case JTokenType.Array:

                        var jArray = (JArray)jToken;
                        var shouldTranslateChild = key as string == "inlines";

                        for (int i = 0; i < jArray.Count; i++)
                        {
                            switchOnJToken(jArray, i, shouldTranslateChild);
                        }

                        break;

                    case JTokenType.String:

                        if (shouldTranslate)
                        {
                            // Store the text to translate as well as the JToken information to apply the translated text to
                            list.Add((jContainer, key, (string)jToken));
                        }

                        break;
                }
            }

            recurseThroughJObject(cardJObject);

            // From Cognitive Services translation documentation:
            // https://docs.microsoft.com/en-us/azure/cognitive-services/translator/quickstart-csharp-translate
            var requestBody = JsonConvert.SerializeObject(list.Select(item => new { item.Text }));

            var result = await _translationClient.TranslateAdaptiveCardAsync(requestBody,userLanguage);

            if (result == null)
            {
                return null;
            }

            for (int i = 0; i < result.Length && i < list.Count; i++)
            {
                var item = list[i];
                var translatedText = result[i]?.Translations?.FirstOrDefault()?.Text;

                if (!string.IsNullOrWhiteSpace(translatedText))
                {
                    // Modify each stored JToken with the translated text
                    item.Container[item.Key] = translatedText;
                }
            }

            // Return the modified JObject representing the Adaptive Card
            return cardJObject;
        }
    }
}
