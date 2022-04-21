using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using RestroQnABot.Models;
using RestroQnABot.Utlities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RestroQnABot.Dialogs
{
    public class QuestionAnsweringDialog : ComponentDialog
    {
        private QuestionAnswerManager _questionAnswerManager;
        private readonly IStatePropertyAccessor<string> _languageStateProperty;
        public QuestionAnsweringDialog(IConfiguration configuration,UserState userState)
        {
            _languageStateProperty = userState.CreateProperty<string>("LanguagePreference");
            _questionAnswerManager = new QuestionAnswerManager(
                new CustomQnAServiceClient(configuration),
                new AdaptiveCardManager(new TranslationManager(new AzureTranslationClient(configuration))));
        }

        public override async Task<DialogTurnResult> BeginDialogAsync(DialogContext outerDc, object options = null, CancellationToken cancellationToken = default)
        {

            var knowleadgeBaseId = options as string;
            var userLangCode = await _languageStateProperty.GetAsync(outerDc.Context, () => TranslationSettings.DefaultLanguage) ?? TranslationSettings.DefaultLanguage;

            //If adaptive card has buttons then get the button values.
            if (outerDc.Context.Activity.Text == null)
            {
                var token = JToken.Parse(outerDc.Context.Activity.Value.ToString());
                outerDc.Context.Activity.Text = token["action"].Value<string>();
            }

            var reply = await _questionAnswerManager.GetAnswer(outerDc.Context.Activity.Text,knowleadgeBaseId);

            return await outerDc.EndDialogAsync(reply, cancellationToken);
        }
    }
}
