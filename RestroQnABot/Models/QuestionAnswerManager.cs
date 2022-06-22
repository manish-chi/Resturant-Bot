using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using RestroQnABot.Interfaces;
using RestroQnABot.Serializable;
using RestroQnABot.Utlities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestroQnABot.Models
{
    public class QuestionAnswerManager
    {
        private IQuestionAnswer _questionAnswer;
        private AdaptiveCardManager _adaptiveCardManager;
        protected string CommonKnowleageBaseId = String.Empty;
        private CustomQnAResponse _response;
        public QuestionAnswerManager(IConfiguration configuration,IQuestionAnswer questionAnswer, AdaptiveCardManager adaptiveCardManager)
        {
            _response = new CustomQnAResponse();
            _questionAnswer = questionAnswer;
            _adaptiveCardManager = adaptiveCardManager;
            CommonKnowleageBaseId = configuration[nameof(CommonKnowleageBaseId)];
        }

        public async Task<IMessageActivity> GetAnswerFromMultipleKb(string question, List<string> knowleadgeBaseSources)
        {

            foreach (var kbSource in knowleadgeBaseSources){

                _response = await _questionAnswer.GetAnswers(question, kbSource);

                if (_response.answers[0].confidenceScore > 0.85)
                {
                    break;
                }
                else {

                    continue;
                }
            }

            return this.GetQnAResponse(question, _response);
        }

        public async Task<IMessageActivity> GetAnswerFromSingleKb(string question, string knowleadgeBaseSource) {

            _response = await _questionAnswer.GetAnswers(question,knowleadgeBaseSource);

            return this.GetQnAResponse(question, _response);
        }

        private IMessageActivity GetQnAResponse(string question, CustomQnAResponse response) {

            if (response.answers.Count() > 0)
            {
                if (response.answers[0].dialog != null)
                {
                    if (response.answers[0].dialog.prompts.Count() > 0)
                    {
                        var attachment = _adaptiveCardManager.buildAdaptiveCard(response.answers[0].questions[0],
                          response.answers[0].answer, response.answers[0].dialog.prompts.ToList());

                        return MessageFactory.Attachment(attachment);
                    }
                    else
                    {

                        return AdaptiveReponseTypeMessage(response);
                    }
                }
                else
                {
                    return AdaptiveReponseTypeMessage(response);
                }
            }
            else
            {
                return MessageFactory.Text(response.answers[0].answer);
            }
        }

        private IMessageActivity AdaptiveReponseTypeMessage(CustomQnAResponse response)
        {
            if (!String.IsNullOrEmpty(response.answers[0].metadata.responsetype)
                        && String.Equals(response.answers[0].metadata.responsetype, "adaptivecard", StringComparison.InvariantCultureIgnoreCase))
            {
                var attachment = _adaptiveCardManager.FormatAdaptiveCard(response.answers[0]);

                return MessageFactory.Attachment(attachment);
            }
            else
            {
                return MessageFactory.Text(response.answers[0].answer);
            }
        }
    }
}
