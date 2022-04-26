using Azure;
using Azure.AI.Language.QuestionAnswering;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RestroQnABot.Interfaces;
using RestroQnABot.Models;
using RestroQnABot.Serializable;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace RestroQnABot.Utlities
{
    public class CustomQnAServiceClient : IQuestionAnswer
    {
        private string _endpoint;
        private string _key;
        private string _projectName;
        private string _productionName;
        public CustomQnAServiceClient(IConfiguration configuration)
        {
            _endpoint = configuration["QnAEndpoint"];
            _key = configuration["QnAKey"];
            _projectName = configuration["ProjectName"];
            _productionName = configuration["ProductionName"];
        }

        public async Task<CustomQnAResponse> GetAnswers(string question, string knowleadgeBaseSource)
        {
            try
            {
               
                string projectName = _projectName;
                string deploymentName = _productionName;

                var sourceFilters = new List<string>();
                sourceFilters.Add($"{knowleadgeBaseSource}");

                var body = new CustomQnARequest()
                {
                    question = question,
                    filters = new Filters()
                    {
                        sourceFilter = sourceFilters.ToArray(),
                        logicalOperation = "AND"
                    }
                };

                var requestBody = JsonConvert.SerializeObject(body);

                using (var client = new HttpClient())
                {
                    using (var request = new HttpRequestMessage())
                    {
                        // Build the request.
                        request.Method = HttpMethod.Post;
                        request.RequestUri = new Uri($"{_endpoint}language/:query-knowledgebases?projectName={_projectName}&deploymentName={_productionName}&api-version=2021-10-01");
                        request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
                        request.Headers.Add("Ocp-Apim-Subscription-Key", _key);
                        request.Headers.Add("Ocp-Apim-Subscription-Region", "eastus");
                        // Send the request and get response.
                        HttpResponseMessage response = await client.SendAsync(request).ConfigureAwait(false);
                        // Read response as a string.
                        string result = await response.Content.ReadAsStringAsync();
                        var deserializedOutput = JsonConvert.DeserializeObject<CustomQnAResponse>(result);
   
                        return deserializedOutput;
                    }
                }

                //Response<AnswersResult> response = await client.GetAnswersAsync(question, project);

                //return response;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
    }
}
