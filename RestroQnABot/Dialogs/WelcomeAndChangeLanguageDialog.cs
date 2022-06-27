using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;
using RestroQnABot.Cards;
using RestroQnABot.Models;
using RestroQnABot.Utlities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace RestroQnABot.Dialogs
{
    public class WelcomeAndChangeLanguageDialog : ComponentDialog
    {
        private IConfiguration _configuration;
        private IStatePropertyAccessor<KnowleadgeSourceData> _knowleadgeBaseAccessor;
        private IStatePropertyAccessor<bool> _changeLanguageAccessor;


        private QuestionAnswerManager _questionAnswerManager;
        private LanguageManager _languageManager;
        private CardManager _cardManager;
        private UserState _userState;

        public string WelcomePrompt { get; }
        public string ChangeLanguagePrompt { get; }

        public WelcomeAndChangeLanguageDialog(IConfiguration configuration, UserState userState, TranslationManager translationManager) : base(nameof(WelcomeAndChangeLanguageDialog))
        {
            this._configuration = configuration;
            this._userState = userState;

            _changeLanguageAccessor = userState.CreateProperty<bool>(nameof(_changeLanguageAccessor));
            _knowleadgeBaseAccessor = userState.CreateProperty<KnowleadgeSourceData>(nameof(KnowleadgeSourceData));

            _questionAnswerManager = new QuestionAnswerManager(configuration, new CustomQnAServiceClient(configuration)
               , new AdaptiveCardManager());

            _languageManager = new LanguageManager(configuration,
                userState, new TranslationManager(new AzureTranslationClient(configuration)));

            _cardManager = new CardManager(configuration, _questionAnswerManager);

            WelcomePrompt = configuration["WelcomePrompt"];
            ChangeLanguagePrompt = configuration["ChangeLanguagePrompt"];

            var steps = new WaterfallStep[] {
                GetChangeLanguagePromptAsync,
                GetWelcomeCardAsync,
            };

            Dialogs.Add(new WaterfallDialog($"{nameof(WelcomeAndChangeLanguageDialog)}Steps", steps));
        }


        private async Task<DialogTurnResult> GetWelcomeCardAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var knowleageSourceData = await _knowleadgeBaseAccessor.GetAsync(stepContext.Context, () => new KnowleadgeSourceData(), cancellationToken);

            if (knowleageSourceData.multiLang)
            {
                ConnectorClient connector = new ConnectorClient(
                    new Uri(stepContext.Context.Activity.ServiceUrl),
                    _configuration["MicrosoftAppId"], _configuration["MicrosoftAppPassword"]
                    );

                await _languageManager.CheckAndSetLanguageAsync(stepContext.Context, cancellationToken);

                var welcomeReply = stepContext.Context.Activity.CreateReply();

                var reply = await _cardManager.WelcomeCard(knowleageSourceData.data[0]);

                welcomeReply.Attachments = reply.Attachments;

                await connector.Conversations.SendToConversationAsync(welcomeReply, cancellationToken);

            }
            else {

                var reply = await _cardManager.WelcomeCard(knowleageSourceData.data[0]);

                await stepContext.Context.SendActivityAsync(reply, cancellationToken);
            }

          

            return await stepContext.EndDialogAsync(null, cancellationToken);
        }

        private async Task<DialogTurnResult> GetChangeLanguagePromptAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var knowleageSourceData = await _knowleadgeBaseAccessor.GetAsync(stepContext.Context, () => new KnowleadgeSourceData(), cancellationToken);

            if (knowleageSourceData.multiLang)
            {
                ConnectorClient connector = new ConnectorClient(
                    new Uri(stepContext.Context.Activity.ServiceUrl),
                    _configuration["MicrosoftAppId"],_configuration["MicrosoftAppPassword"] 
                    );

                var reply = await _cardManager.ChangeLanguageCard(knowleageSourceData.data[0]);

                var changeLangReply = stepContext.Context.Activity.CreateReply();

                changeLangReply.Attachments = reply.Attachments;

                await connector.Conversations.SendToConversationAsync(changeLangReply, cancellationToken);

                return Dialog.EndOfTurn;
            }
            else
            {
                return await stepContext.NextAsync(null,cancellationToken);
            }
        }

    }
}
