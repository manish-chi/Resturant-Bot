using AdaptiveCards;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using RestroQnABot.Models;
using RestroQnABot.Utlities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RestroQnABot.Middlewares
{
    public class TranslationMiddleWare : IMiddleware
    {
        private IConfiguration _configuration;
        private readonly IStatePropertyAccessor<string> _languageStateProperty;
        private AzureTranslationClient _translationClient;
        private TranslationManager _translationManager;
        private UserState _userState;
        public TranslationMiddleWare(IConfiguration configuration, UserState userState)
        {
            _userState = userState;
            _configuration = configuration;
            _translationClient = new AzureTranslationClient(configuration);
            _translationManager = new TranslationManager(_translationClient);
            _languageStateProperty = userState.CreateProperty<string>("LanguagePreference");
        }

        public async Task OnTurnAsync(ITurnContext turnContext, NextDelegate next, CancellationToken cancellationToken = default)
        {
            try
            {
                var languageAccessor = _userState.CreateProperty<Language>(nameof(Language));

                var isMultiLang = _configuration["MultiLang"];

                //If multilanguage is true
                if (Convert.ToBoolean(isMultiLang))
                {
                    if (turnContext.Activity.Type == ActivityTypes.Message)
                    {
                        if (!String.IsNullOrEmpty(turnContext.Activity.Text))
                        {
                            var translate = await ShouldTranslateAsync(turnContext, cancellationToken);

                            if (translate)
                            {
                                if (!turnContext.Activity.Text.Equals(TranslationSettings.DefaultLanguage))
                                {
                                    turnContext.Activity.Text = await _translationClient.TranslateTextRequestAsync(turnContext.Activity.Text, TranslationSettings.DefaultLanguage);
                                }
                            }
                        }
                    }

                   

                   

                    turnContext.OnUpdateActivity(async (newContext, activity, nextUpdate) =>
                    {
                        string userLanguage = await _languageStateProperty.GetAsync(turnContext, () => TranslationSettings.DefaultLanguage) ?? TranslationSettings.DefaultLanguage;

                        bool shouldTranslate = userLanguage != TranslationSettings.DefaultLanguage;

                        // Translate messages sent to the user to user language
                        if (activity.Type == ActivityTypes.Message)
                        {
                            if (shouldTranslate)
                            {
                                await TranslateMessageActivityAsync(activity.AsMessageActivity(), TranslationSettings.DefaultLanguage);
                            }
                        }
                        return await nextUpdate();
                    });

                    turnContext.OnSendActivities(async (newContext, activities, nextSend) =>
                    {
                        string userLanguage = await _languageStateProperty.GetAsync(turnContext, () => TranslationSettings.DefaultLanguage) ?? TranslationSettings.DefaultLanguage;

                        bool shouldTranslate = userLanguage != TranslationSettings.DefaultLanguage;

                        // Translate messages sent to the user to user language
                        if (shouldTranslate)
                        {
                            List<Task> tasks = new List<Task>();

                            foreach (Microsoft.Bot.Schema.Activity currentActivity in activities.Where(a => a.Type == ActivityTypes.Message))
                            {
                                tasks.Add(TranslateMessageActivityAsync(currentActivity.AsMessageActivity(), userLanguage));
                            }

                            if (tasks.Any())
                            {
                                await Task.WhenAll(tasks).ConfigureAwait(false);
                            }
                        }
                        return await nextSend();
                    });
                }
            }
            catch (Exception ex)
            {   //********************************
                Debug.WriteLine(ex.Message);
                //throw new Exception(Statements.ErrorStatement);

            }

            await next(cancellationToken).ConfigureAwait(false);
        }

        private async Task TranslateMessageActivityAsync(IMessageActivity activity, string targetLocale, CancellationToken cancellationToken = default(CancellationToken))
        {

            if (activity.Type == ActivityTypes.Message)
            {
                if (activity.Attachments != null)
                {
                    if (activity.Attachments.Count() > 0)
                    {
                        object translatedCard = await _translationManager.TranslateAdaptiveCard(activity.Attachments[0].Content, targetLocale);

                        var card = AdaptiveCard.FromJson(translatedCard.ToString()).Card;

                        activity.Attachments.Add(new Attachment()
                        {
                            ContentType = "application/vnd.microsoft.card.adaptive",
                            Content = card,
                        });

                    }
                    else
                    {
                        activity.Text = await _translationClient.TranslateTextRequestAsync(activity.Text, targetLocale);
                    }
                }
                else
                {
                    activity.Text = await _translationClient.TranslateTextRequestAsync(activity.Text, targetLocale);
                }
            }
        }


        private async Task<bool> ShouldTranslateAsync(ITurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken))
        {

                string userLanguage = await _languageStateProperty.GetAsync(turnContext, () => TranslationSettings.DefaultLanguage, cancellationToken) ?? TranslationSettings.DefaultLanguage;
                return userLanguage != TranslationSettings.DefaultLanguage;
        }
    }
}
