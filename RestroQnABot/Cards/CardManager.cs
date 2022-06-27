using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;
using RestroQnABot.Models;
using System.Threading.Tasks;

namespace RestroQnABot.Cards
{
    public class CardManager
    {
        private readonly string _welcomePrompt;
        private readonly string _changeLanguagePrompt;
        private QuestionAnswerManager _questionAnswerManager;
        public CardManager(IConfiguration configuration,QuestionAnswerManager questionAnswerManager)
        {
            _questionAnswerManager = questionAnswerManager;
            _welcomePrompt = configuration["WelcomePrompt"];
            _changeLanguagePrompt = configuration["ChangeLanguagePrompt"];
        }

        public async Task<IMessageActivity> ChangeLanguageCard(string knowleageBaseSource)
        {
            var reply = await _questionAnswerManager.GetAnswerFromSingleKb(_changeLanguagePrompt,knowleageBaseSource);
            return reply;
        }

        public async Task<IMessageActivity> WelcomeCard(string knowleageBaseSource) {

            var reply = await _questionAnswerManager.GetAnswerFromSingleKb(_welcomePrompt, knowleageBaseSource);
            return reply;
        }
    }
}
