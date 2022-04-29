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

namespace RestroQnABot.Bots
{
    public class RestroBot<T> : ActivityHandler where T : Dialog
    {
        protected IConfiguration configuration;
        protected Dialog dialog;
        private QuestionAnswerManager _questionAnswerManager;
        private LanguageManager _languageManager;
        private IStatePropertyAccessor<bool> _welcomeAccessor;
        private readonly IStatePropertyAccessor<string> _languageAccessor;
        protected string WelcomePrompt = String.Empty;
        protected string ChangeLanguagePrompt = String.Empty;
        protected UserState userState;
        protected ConversationState conversationState;
        public RestroBot(IConfiguration configuration, T dialog, UserState userState, ConversationState conversationState)
        {
            this.configuration = configuration;
            this.userState = userState;
            this.dialog = dialog;

            this.conversationState = conversationState;
            _welcomeAccessor = userState.CreateProperty<bool>("welcome");
            _languageAccessor = userState.CreateProperty<string>("LanguagePreference");

            _languageManager = new LanguageManager(configuration, userState, new TranslationManager(new AzureTranslationClient(configuration)));
            _questionAnswerManager = new QuestionAnswerManager(configuration,new CustomQnAServiceClient(configuration)
                , new AdaptiveCardManager());

            WelcomePrompt = configuration["WelcomePrompt"];
            ChangeLanguagePrompt = configuration["ChangeLanguagePrompt"];

        }


        public override async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default)
        {
        
            await base.OnTurnAsync(turnContext, cancellationToken);

            // Save any state changes that might have occurred during the turn.
            await userState.SaveChangesAsync(turnContext, false, cancellationToken);
            await conversationState.SaveChangesAsync(turnContext, false, cancellationToken);
        }

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            var isFirstMessage = await _welcomeAccessor.GetAsync(turnContext, () => new bool(), cancellationToken);

            if (isFirstMessage && Convert.ToBoolean(configuration["MultiLang"]))
            {

                await _languageManager.CheckAndSetLanguageAsync(turnContext, cancellationToken);

                var userLangCode = await _languageAccessor.GetAsync(turnContext, () => TranslationSettings.DefaultLanguage) ?? TranslationSettings.DefaultLanguage;

                var welcomeReply = await this.WelcomeCard();

                await turnContext.SendActivityAsync(welcomeReply, cancellationToken);

                //set the welcomeAccessor to false
                await _welcomeAccessor.SetAsync(turnContext, false, cancellationToken);
            }
            else
            {
                //Save Dialog Stack
                await dialog.RunAsync(turnContext, conversationState.CreateProperty<DialogState>("DialogState"), cancellationToken);
            }


        }

        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            var userLangCode = await _languageAccessor.GetAsync(turnContext, () => TranslationSettings.DefaultLanguage) ?? TranslationSettings.DefaultLanguage;

            foreach (var member in membersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    if (Convert.ToBoolean(configuration["MultiLang"]))
                    {
                        var languageText = ChangeLanguagePrompt;
                        var reply = await _questionAnswerManager.GetAnswer(languageText);
                        await turnContext.SendActivityAsync(reply, cancellationToken);

                        await _welcomeAccessor.SetAsync(turnContext, true, cancellationToken);
                    }
                    else
                    {
                        var reply = await this.WelcomeCard();
                        await turnContext.SendActivityAsync(reply, cancellationToken);
                    }
                }
            }
        }

        private async Task<IMessageActivity> WelcomeCard()
        {
            //showing welcome card after lang selection.
            var welcomeText = WelcomePrompt;
            var reply = await _questionAnswerManager.GetAnswer(welcomeText);
            return reply;
        }
    }
}
