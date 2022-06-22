// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with Bot Builder V4 SDK Template for Visual Studio CoreBot v4.15.2

using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Builder.TraceExtensions;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RestroQnABot.Middlewares;
using RestroQnABot.Models;
using RestroQnABot.Utlities;

namespace RestroQnABot
{
    public class AdapterWithErrorHandler : CloudAdapter
    {
        private AppSettings _appSettings;
        private UserState _userState;
        private IStatePropertyAccessor<KnowleadgeBaseSettings> _knowleadgeBaseAccessor;
        public AdapterWithErrorHandler(BotFrameworkAuthentication auth,
            UserState userState,InputTypeMiddleWare inputTypeMiddleware,TranslationMiddleWare translationMiddleWare,IOptions<AppSettings> appSettings, ILogger<IBotFrameworkHttpAdapter> logger,IConfiguration configuration)
            : base(auth, logger)
        {
            _userState = userState;
            _appSettings = appSettings.Value;

            _knowleadgeBaseAccessor = userState.CreateProperty<KnowleadgeBaseSettings>(nameof(KnowleadgeBaseSettings));


            var questionAnswerManager = new QuestionAnswerManager(configuration,new CustomQnAServiceClient(configuration), new AdaptiveCardManager());
            Use(inputTypeMiddleware);
            Use(translationMiddleWare);

            OnTurnError = async (turnContext, exception) =>
            {
                if (_appSettings.Environment.Equals(Environments.Development))
                {
                    // Log any leaked exception from the application.
                    logger.LogError(exception, $"[OnTurnError] unhandled error : {exception.Message}");

                    // Send a message to the user
                    await turnContext.SendActivityAsync("The bot encountered an error or bug.");
                    await turnContext.SendActivityAsync(exception.Message);

                    // Send a trace activity, which will be displayed in the Bot Framework Emulator
                    await turnContext.TraceActivityAsync("OnTurnError Trace", exception.Message, "https://www.botframework.com/schemas/error", "TurnError");
                }
                else {

                    var knowleadgeBaseSettings = await _knowleadgeBaseAccessor.GetAsync(turnContext, () => new KnowleadgeBaseSettings());

                    var kbSources = knowleadgeBaseSettings.KnowleageBaseSource;

                    var reply = await questionAnswerManager.GetAnswerFromSingleKb("Error", kbSources[0]);
                    // Send a message to the user
                    await turnContext.SendActivityAsync(reply);
                }
            };
        }
    }
}
