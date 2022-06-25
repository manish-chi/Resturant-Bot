using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;
using RestroQnABot.ConstantsLitrals;
using RestroQnABot.Models;
using RestroQnABot.Utlities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RestroQnABot.Dialogs
{
    public class IntermediateDialog : CancelAndHelpDialog
    {
        private LanguageManager _languageManager;
        private IStatePropertyAccessor<bool> _welcomeAccessor;
        private  IStatePropertyAccessor<KnowleadgeBaseSettings> _knowleadgeBaseAccessor;
        private string knowleageBaseSource;

        public IntermediateDialog(IConfiguration configuration, UserState userState, TranslationManager translationManager) : base(nameof(IntermediateDialog), configuration, userState, translationManager)
        {
            _languageManager = new LanguageManager(configuration, userState, translationManager);

            _welcomeAccessor = userState.CreateProperty<bool>("welcome");

            _knowleadgeBaseAccessor = userState.CreateProperty<KnowleadgeBaseSettings>(nameof(KnowleadgeBaseSettings));

            var steps = new WaterfallStep[] {
                GetCommonBotAnswerAsync,
                GetNoAnswerFoundAsync,
                FinalStepAsync,
            };
            Dialogs.Add(new WaterfallDialog("waterfallsteps", steps));
            Dialogs.Add(new QuestionAnsweringDialog(configuration,userState));
        }

        private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var reply = stepContext.Result as IMessageActivity;

            await stepContext.Context.SendActivityAsync(reply, cancellationToken);

            return await stepContext.EndDialogAsync(null, cancellationToken);
        }

        //private async Task<DialogTurnResult> GetNoAnswerFoundAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        //{
        //    stepContext.Context.Activity.Text = Constant.noAnswerFound;

        //    return await this.ForwardToQnADialog(stepContext,cancellationToken);
        //}

        private async Task<DialogTurnResult> GetCommonBotAnswerAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            bool isLangPresent = _languageManager.IfLangChangePresent(stepContext.Context);

            if (isLangPresent)
            {
                await _languageManager.CheckAndSetLanguageAsync(stepContext.Context, cancellationToken);

                return await stepContext.EndDialogAsync(null, cancellationToken);
            }
            else {

                var knowleadgeBaseSettings = await _knowleadgeBaseAccessor.GetAsync(stepContext.Context, () => new KnowleadgeBaseSettings(), cancellationToken);

                var kbSources = knowleadgeBaseSettings.KnowleageBaseSource;

                return await stepContext.BeginDialogAsync(nameof(QuestionAnsweringDialog),kbSources, cancellationToken);
            }
        }

        private async Task<DialogTurnResult> GetNoAnswerFoundAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var knowleadgeBaseSettings = await _knowleadgeBaseAccessor.GetAsync(stepContext.Context, () => new KnowleadgeBaseSettings(), cancellationToken);

            var kbSources = knowleadgeBaseSettings.KnowleageBaseSource;

            //sending only first source if the answer is not found.
            return await this.ForwardToQnADialog(stepContext, cancellationToken,kbSources[0]); 
        }

        private async Task<DialogTurnResult> ForwardToQnADialog(WaterfallStepContext stepContext, CancellationToken cancellationToken, String knowleadgeBaseSource = null) {

            var reply = stepContext.Result as IMessageActivity;

            if (!string.IsNullOrEmpty(reply.Text))
            { 
                if (reply.Text.Equals(Constant.noAnswerFound, StringComparison.InvariantCultureIgnoreCase))
                {
                    stepContext.Context.Activity.Text = Constant.noAnswerFound;

                    return await stepContext.BeginDialogAsync(nameof(QuestionAnsweringDialog),knowleadgeBaseSource,cancellationToken);
                }
                else
                {
                    await stepContext.Context.SendActivityAsync(reply, cancellationToken);

                    return await stepContext.EndDialogAsync(null, cancellationToken);
                }

            }
            else
            { //for adaptive cards
                await stepContext.Context.SendActivityAsync(reply, cancellationToken);

                return await stepContext.EndDialogAsync(null, cancellationToken);
            }
        }
    }
}
