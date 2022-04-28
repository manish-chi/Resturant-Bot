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

            return this.ReturnAdaptiveCardAttachement(card);
        }


        private Attachment ReturnAdaptiveCardAttachement(AdaptiveCard card) {

            var adaptiveCardAttachment = new Attachment()
            {
                ContentType = "application/vnd.microsoft.card.adaptive",
                Content = card,
            };

            return adaptiveCardAttachment;
        }

        public void SetAdaptiveButtonValueToText(IMessageActivity activity)
        {
            if (String.IsNullOrEmpty(activity.Text) && activity.Value != null) //its an adaptive card
            {
                var token = JToken.Parse(activity.Value.ToString());

                if (token.SelectToken("action") != null) {

                    var action = token["action"].Value<string>() ?? null;

                    if (!string.IsNullOrEmpty(action))
                    {
                        activity.Text = action;
                    }
                }
            }
        }

        public Attachment buildAdaptiveCard(string question,string title,List<Prompt> prompts) {

            var adaptiveElements = new List<AdaptiveElement>();
            var submitActions = new List<AdaptiveAction>();

            var textBlock = new AdaptiveTextBlock() {
                Text = title,
                Wrap = true,
            };

            foreach(var prompt in prompts) {

                submitActions.Add(new AdaptiveSubmitAction()
                {
                    Title = prompt.displayText,
                    Id = prompt.displayText,
                    Data = $"{prompt.displayText}",
                    DataJson = JsonConvert.SerializeObject(new { action = $"{question} {prompt.displayText}"})
                });
            }

            adaptiveElements.Add(textBlock);

            var card = new AdaptiveCard("1.3");
            card.Body = adaptiveElements;
            card.Actions = submitActions;
            card.Type = AdaptiveCard.TypeName;

            return this.ReturnAdaptiveCardAttachement(card);
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
