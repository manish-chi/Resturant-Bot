using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RestroQnABot.Interfaces;
using RestroQnABot.Serializable;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace RestroQnABot.Utlities
{
    public class AzureTranslationClient : ITranslationClient
    {
        private string _translationKey = String.Empty;
        public AzureTranslationClient(IConfiguration configuration) {
            _translationKey = configuration["translationKey"];
        }
        public async Task<string> DetectLanguageFromTextAsync(string utterance)
        {
            var textInputs = new List<TextInput>();
            textInputs.Add(new TextInput() { Text = utterance });

            try
            {      
                object[] body = new object[] { new { Text = utterance } };
                var requestBody = JsonConvert.SerializeObject(body);

                using (var client = new HttpClient())
                {
                    using (var request = new HttpRequestMessage())
                    {
                        // Build the request.
                        request.Method = HttpMethod.Post;
                        request.RequestUri = new Uri("https://api.cognitive.microsofttranslator.com/detect?api-version=3.0");
                        request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
                        request.Headers.Add("Ocp-Apim-Subscription-Key", "f146f7f3118d412aa7b076792b317ed8");
                        request.Headers.Add("Ocp-Apim-Subscription-Region", "eastus");
                        // Send the request and get response.
                        HttpResponseMessage response = await client.SendAsync(request).ConfigureAwait(false);
                        // Read response as a string.
                        string result = await response.Content.ReadAsStringAsync();
                        var deserializedOutput = JsonConvert.DeserializeObject<LanguageResult[]>(result);

                        return deserializedOutput[0].language;
                    }
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public async Task<string> TranslateTextRequestAsync(string utterance, string languageCode)
        {
            try
            {
                object[] body = new object[] { new { Text = utterance } };
                var requestBody = JsonConvert.SerializeObject(body);

                using (var client = new HttpClient())
                {
                    using (var request = new HttpRequestMessage())
                    {
                        // Build the request.
                        request.Method = HttpMethod.Post;
                        request.RequestUri = new Uri($"https://api.cognitive.microsofttranslator.com/translate?api-version=3.0&to={languageCode.Trim()}");
                        request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
                        request.Headers.Add("Ocp-Apim-Subscription-Key", _translationKey);
                        request.Headers.Add("Ocp-Apim-Subscription-Region", "eastus");
                        // Send the request and get response.
                        HttpResponseMessage response = await client.SendAsync(request).ConfigureAwait(false);
                        // Read response as a string.
                        string result = await response.Content.ReadAsStringAsync();
                        TranslationResult[] deserializedOutput = JsonConvert.DeserializeObject<TranslationResult[]>(result);

                        return deserializedOutput[0].Translations[0].Text;
                    }
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<TranslationResult[]> TranslateAdaptiveCardAsync(string requestBody,string languageCode)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    using (var request = new HttpRequestMessage())
                    {
                        // Build the request.
                        request.Method = HttpMethod.Post;
                        request.RequestUri = new Uri($"https://api.cognitive.microsofttranslator.com/translate?api-version=3.0&to={languageCode.Trim()}");
                        request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
                        request.Headers.Add("Ocp-Apim-Subscription-Key", _translationKey);
                        request.Headers.Add("Ocp-Apim-Subscription-Region", "eastus");
                        // Send the request and get response.
                        HttpResponseMessage response = await client.SendAsync(request).ConfigureAwait(false);
                        // Read response as a string.
                        string result = await response.Content.ReadAsStringAsync();
                        TranslationResult[] deserializedOutput = JsonConvert.DeserializeObject<TranslationResult[]>(result);

                        return deserializedOutput;
                    }
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
