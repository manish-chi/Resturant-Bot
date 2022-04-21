using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;
using RestroQnABot.Models;
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
        public IntermediateDialog(IConfiguration configuration, UserState userState, TranslationManager translationManager) : base(nameof(IntermediateDialog), configuration, userState, translationManager)
        {
            _languageManager = new LanguageManager(configuration, userState, translationManager);

            var steps = new WaterfallStep[] {
                GetCommonBotAnswerAsync,
                GetSpecificBotAnswerAsync,
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

        private async Task<DialogTurnResult> GetNoAnswerFoundAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var reply = stepContext.Result as IMessageActivity;

            if (reply.Text.Equals("No Answer Found", StringComparison.InvariantCultureIgnoreCase))
            {
                return await stepContext.BeginDialogAsync(nameof(QuestionAnsweringDialog), "NoAnswerFound", cancellationToken);
            }
            else
            {
                await stepContext.Context.SendActivityAsync(reply, cancellationToken);
                return await stepContext.EndDialogAsync(null, cancellationToken);
            }           
        }

        private async Task<DialogTurnResult> GetCommonBotAnswerAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            bool isLangPresent = _languageManager.IfLangChangePresent(stepContext.Context);

            if (isLangPresent)
            {
                await _languageManager.CheckAndSetLanguageAsync(stepContext.Context, cancellationToken);

                return await stepContext.EndDialogAsync(null, cancellationToken);
            }
            else {
                 
                return await stepContext.BeginDialogAsync(nameof(QuestionAnsweringDialog), "common", cancellationToken);

               
            }
        }

        private async Task<DialogTurnResult> GetSpecificBotAnswerAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var reply = stepContext.Result as IMessageActivity;

            if (!string.IsNullOrEmpty(reply.Text))
            {
                if (reply.Text.Equals("No Answer Found", StringComparison.InvariantCultureIgnoreCase))
                {
                    var knowleadgeBaseID = stepContext.Context.Activity.From.Id;

                    return await stepContext.BeginDialogAsync(nameof(QuestionAnsweringDialog), "Bot-Hyderabad", cancellationToken);
                }
                else {
                    await stepContext.Context.SendActivityAsync(reply, cancellationToken);

                    return await stepContext.EndDialogAsync(null, cancellationToken);
                }
            }
            else {
                await stepContext.Context.SendActivityAsync(reply, cancellationToken);

                return await stepContext.EndDialogAsync(null, cancellationToken);
            }
        }
    }
}
