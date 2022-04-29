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
        public QuestionAnswerManager(IConfiguration configuration,IQuestionAnswer questionAnswer, AdaptiveCardManager adaptiveCardManager)
        {
            _questionAnswer = questionAnswer;
            _adaptiveCardManager = adaptiveCardManager;
            CommonKnowleageBaseId = configuration[nameof(CommonKnowleageBaseId)];
        }

        public async Task<IMessageActivity> GetAnswer(string question, string knowleadgeBaseId = null)
        {

            knowleadgeBaseId = String.IsNullOrEmpty(knowleadgeBaseId) ? CommonKnowleageBaseId : knowleadgeBaseId;

            var response = await _questionAnswer.GetAnswers(question, knowleadgeBaseId);

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
                    else {

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
