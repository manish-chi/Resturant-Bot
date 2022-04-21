using Microsoft.Bot.Builder;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using RestroQnABot.Utlities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RestroQnABot.Models
{
    public class LanguageManager
    {
        private IConfiguration _configuration;
        private TranslationManager _translationManager;
        private readonly IStatePropertyAccessor<string> _languageStateProperty;


        public LanguageManager(IConfiguration configuration, UserState userState, TranslationManager translationManager)
        {
            _configuration = configuration;
            _translationManager = translationManager;
            _languageStateProperty = userState.CreateProperty<string>("LanguagePreference");
        }


        public bool IfLangChangePresent(ITurnContext context) {
            var langCodeFromQnA = String.Empty;
            var languageTitle = String.Empty;

            if (context.Activity.Text == null)
            {
                var token = JToken.Parse(context.Activity.Value.ToString());
                langCodeFromQnA = token["code"].Value<string>().Trim();
                languageTitle = token["language"].Value<string>().Trim();
            }

            if (!String.IsNullOrEmpty(langCodeFromQnA) && !String.IsNullOrEmpty(languageTitle))
            {
                return true;
            }
            else {
                return false;
            }

        }



        public async Task CheckAndSetLanguageAsync(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            var langCodeFromQnA = String.Empty;
            var languageTitle = String.Empty;
            try
            {
                if (turnContext.Activity.Text == null)
                {
                    var token = JToken.Parse(turnContext.Activity.Value.ToString());
                    langCodeFromQnA = token["code"].Value<string>().Trim();
                    languageTitle = token["language"].Value<string>().Trim();
                }

                var userLangCode = await this._translationManager.DetectLanguageAsync(languageTitle);

                if (userLangCode.Equals(langCodeFromQnA, StringComparison.InvariantCultureIgnoreCase))
                {
                    await this.SetLanguageAsync(turnContext, cancellationToken,languageTitle, userLangCode);
                }
                else
                {
                    await _languageStateProperty.SetAsync(turnContext, TranslationSettings.DefaultLanguage, cancellationToken);
                    await turnContext.SendActivityAsync(MessageFactory.Text($"Default language has been set to **English**.  \nYou can change language anytime by typing in *'change language'*."));
                }
            }
            catch (InvalidCastException ex)
            {
                Debug.WriteLine(ex.Message);
                //await turnContext.SendActivityAsync(MessageFactory.Text(Statements.ErrorStatement), cancellationToken);
            }
        }

        private async Task SetLanguageAsync(ITurnContext turnContext, CancellationToken cancellationToken,string languageName,string userLangCode)
        {
            await _languageStateProperty.SetAsync(turnContext, userLangCode, cancellationToken);
            await turnContext.SendActivityAsync(MessageFactory.Text($"You have chosen **'{languageName}'**.  \nYou can change language anytime by typing in *'change language'*."));
        }
    }
}
