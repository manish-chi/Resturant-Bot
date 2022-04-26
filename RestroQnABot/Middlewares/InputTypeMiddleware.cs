using Microsoft.Bot.Builder;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using RestroQnABot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RestroQnABot.Middlewares
{
    public class InputTypeMiddleWare : IMiddleware
    {
        private IConfiguration _configuration;
        private AdaptiveCardManager _adaptiveCard;
        private LanguageManager _languageManager;

        public InputTypeMiddleWare(IConfiguration configuration,UserState userState,TranslationManager translationManager)
        {
            _configuration = configuration;
            _adaptiveCard = new AdaptiveCardManager();
            _languageManager = new LanguageManager(configuration,userState,translationManager);
        }

        public async Task OnTurnAsync(ITurnContext turnContext, NextDelegate next, CancellationToken cancellationToken = default(CancellationToken))
        {

            //check if the input type is adaptive card or not.
            bool isAdaptive = _adaptiveCard.isInputTextAdaptiveCardOrNot(turnContext.Activity);

            if (isAdaptive)
            {
               bool isLangAdaptive =  _adaptiveCard.isLangAdaptiveCard(turnContext.Activity);

                if (isLangAdaptive)
                {
                    await next.Invoke(cancellationToken);
                }
                else {
                    var token = JToken.Parse(turnContext.Activity.Value.ToString());
                    turnContext.Activity.Text = token["action"].Value<string>();
                    await next.Invoke(cancellationToken);
                }
            }
            else {
                await next.Invoke(cancellationToken);
            }
        }
    }
}
