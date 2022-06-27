// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with Bot Builder V4 SDK Template for Visual Studio EchoBot v4.15.2

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AdaptiveCards;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using RestroQnABot.Models;
using RestroQnABot.Utlities;
using RestroQnABot.ConstantsLitrals;
using Newtonsoft.Json;
using System.Linq;
using Microsoft.Bot.Builder.Azure.Blobs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using RestroQnABot.Dialogs;

namespace RestroQnABot.Bots
{
    public class RestroBot : IBot
    {
        protected Dialog dialog;
        private QuestionAnswerManager _questionAnswerManager;
        private LanguageManager _languageManager;
        protected UserState userState;
        protected ConversationState conversationState;

        private DialogSet Dialogs { get; set; }

        protected IConfiguration configuration;
        private IStatePropertyAccessor<bool> _welcomeAccessor;
        private IStatePropertyAccessor<string> _languageAccessor;
        private IStatePropertyAccessor<KnowleadgeSourceData> _knowleadgeBaseAccessor;


        protected string WelcomePrompt = String.Empty;
        protected string ChangeLanguagePrompt = String.Empty;
      
        public RestroBot(IConfiguration configuration, UserState userState, ConversationState conversationState,TranslationManager translationManager)
        {
            this.configuration = configuration;
            this.userState = userState;
            this.conversationState = conversationState;
            var dialogStateAccessor = conversationState.CreateProperty<DialogState>(nameof(DialogState));

            Dialogs = new DialogSet(dialogStateAccessor);
            Dialogs.Add(new WelcomeAndChangeLanguageDialog(configuration,userState,translationManager));
            Dialogs.Add(new IntermediateDialog(configuration, userState, translationManager));

           
            _welcomeAccessor = userState.CreateProperty<bool>("welcome");
            _languageAccessor = userState.CreateProperty<string>("LanguagePreference");
            _knowleadgeBaseAccessor = userState.CreateProperty<KnowleadgeSourceData>(nameof(KnowleadgeSourceData));


            _questionAnswerManager = new QuestionAnswerManager(configuration, new CustomQnAServiceClient(configuration)
               , new AdaptiveCardManager());

            _languageManager = new LanguageManager(configuration, userState,
                new TranslationManager(new AzureTranslationClient(configuration)));

            WelcomePrompt = configuration["WelcomePrompt"];
            ChangeLanguagePrompt = configuration["ChangeLanguagePrompt"];

        }


        public async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default)
        {
            // Create a dialog context
            var dc = await Dialogs.CreateContextAsync(turnContext);

            if (turnContext.Activity.Type == ActivityTypes.Message)
            {
                // If there's no active dialog, begin the parent dialog
                if (dc.ActiveDialog == null)
                {
                    await dc.BeginDialogAsync(nameof(IntermediateDialog), null, cancellationToken);
                }
                else
                {
                    await dc.ContinueDialogAsync();
                }
            }
            else if (turnContext.Activity.Type == ActivityTypes.Event) {

                await this.OnEventActivityAsync(turnContext, cancellationToken);
            }

            // Save any state changes that might have occurred during the turn.

            await userState.SaveChangesAsync(turnContext, false, cancellationToken);
            await conversationState.SaveChangesAsync(turnContext, false, cancellationToken);
        }


        protected async Task OnEventActivityAsync(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            switch (turnContext.Activity.Name) {

                case "webchat/join":
                    var kbData = await _knowleadgeBaseAccessor.GetAsync(turnContext, () => new KnowleadgeSourceData(), cancellationToken);
                    if (kbData.data == null){
                        var knowleageBaseData = JsonConvert.DeserializeObject<KnowleadgeSourceData>(turnContext.Activity.Value.ToString());
                        await _knowleadgeBaseAccessor.SetAsync(turnContext, knowleageBaseData, cancellationToken);
                        var dc = await Dialogs.CreateContextAsync(turnContext);
                        await dc.BeginDialogAsync(nameof(WelcomeAndChangeLanguageDialog), null, cancellationToken);

                    }
                    break;
            }
        }
    }
}
