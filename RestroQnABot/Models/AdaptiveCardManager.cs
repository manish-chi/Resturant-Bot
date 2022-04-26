using AdaptiveCards;
using Azure.AI.Language.QuestionAnswering;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestroQnABot.Models;
using RestroQnABot.Serializable;
using RestroQnABot.Utlities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestroQnABot.Models
{
    public class AdaptiveCardManager
    {
        public AdaptiveCardManager()
        {

        }

        public Attachment FormatAdaptiveCard(Answer answer)
        {
            // Parse the JSON 
            AdaptiveCardParseResult result = AdaptiveCard.FromJson(answer.answer);
            // Get card from result

            var card = result.Card;

            var adaptiveCardAttachment = new Attachment()
            {
                ContentType = "application/vnd.microsoft.card.adaptive",
                Content = card,
            };
            return adaptiveCardAttachment;
        }

        public bool isInputTextAdaptiveCardOrNot(IMessageActivity activity)
        {
            if (String.IsNullOrEmpty(activity.Text) && activity.Value != null) //its an adaptive card
            {
                return true;
            }
            else {
                return false;
            }
        }

        public bool isLangAdaptiveCard(IMessageActivity activity)
        { 
            if (activity.Value != null)
            {
                string userSelectedLang = String.Empty;
                var token = JToken.Parse(activity.Value.ToString());
                userSelectedLang = token["code"].Value<string>().Trim();
                if (!String.IsNullOrEmpty(userSelectedLang))
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
            else
            {
                return false;
            }
        }
    }
}
