using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
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
            _questionAnswerManager = new QuestionAnswerManager(configuration,
                new CustomQnAServiceClient(configuration),
                new AdaptiveCardManager());
        }

        public override async Task<DialogTurnResult> BeginDialogAsync(DialogContext outerDc, object options = null, CancellationToken cancellationToken = default)
        {
            Activity reply = new Activity();

            var knowleageBaseCollection = options as List<string>;

            if (knowleageBaseCollection.Count() > 0)
            {
                 reply = (Activity) await _questionAnswerManager.GetAnswerFromMultipleKb(outerDc.Context.Activity.Text, knowleageBaseCollection);

            }
            else {
                reply = (Activity) await _questionAnswerManager.GetAnswerFromSingleKb(outerDc.Context.Activity.Text, knowleageBaseCollection[0]);
            }

            return await outerDc.EndDialogAsync(reply, cancellationToken);
        }
    }
}
