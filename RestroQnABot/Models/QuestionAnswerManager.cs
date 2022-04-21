using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
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
        public QuestionAnswerManager(IQuestionAnswer questionAnswer, AdaptiveCardManager adaptiveCardManager)
        {
            _questionAnswer = questionAnswer;
            _adaptiveCardManager = adaptiveCardManager;
        }

        public async Task<IMessageActivity> GetAnswer(string question, string knowleadgeBaseId)
        {

            knowleadgeBaseId = String.IsNullOrEmpty(knowleadgeBaseId) ? "common" : knowleadgeBaseId;

            var response = await _questionAnswer.GetAnswers(question, knowleadgeBaseId);

            if (response.answers.Count() > 0)  //response.Answers[0].Metadata.GetValueOrDefault("responsetype").Equals("adaptivecard"))
            {
                if (!String.IsNullOrEmpty(Convert.ToString(response.answers[0].metadata)))
                {
                    var card = _adaptiveCardManager.FormatAdaptiveCard(response.answers[0]);

                    return MessageFactory.Attachment(card);
                }
                else
                {
                    return MessageFactory.Text(response.answers[0].answer);
                }
            }
            else
            {
                return MessageFactory.Text(response.answers[0].answer);
            }
}
    }
}
