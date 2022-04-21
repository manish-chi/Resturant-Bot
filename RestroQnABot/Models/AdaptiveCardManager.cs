using AdaptiveCards;
using Azure.AI.Language.QuestionAnswering;
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
        private List<AdaptiveAction> _actions;
        private TranslationManager _translationManager;

        public AdaptiveCardManager(TranslationManager translationManager)
        {
            _translationManager = translationManager;
            _actions = new List<AdaptiveAction>();
        }

        public Attachment FormatAdaptiveCard(Answer answer)
        {
            AdaptiveCard card = null;
            // Parse the JSON 
            AdaptiveCardParseResult result = AdaptiveCard.FromJson(answer.answer);
            // Get card from result

            card = result.Card;

            var token = JToken.Parse(Convert.ToString(answer.metadata)) as JToken;
            string lang = token["language"].Value<string>().Trim();

            //if (answer.Metadata.Keys.Any(x => x.Equals("Language", StringComparison.InvariantCultureIgnoreCase)))
            //{
            //    _actions = card.Body.Select(x=>x..Where(x => x.Id.ToLower().Trim()  != userlangCode.ToLower().Trim()).ToList();

            //    card.Actions = _actions;
            //}

            var adaptiveCardAttachment = new Attachment()
            {
                ContentType = "application/vnd.microsoft.card.adaptive",
                Content = card,
            };
            return adaptiveCardAttachment;
        }
    }
}
