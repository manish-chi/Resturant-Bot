using Microsoft.Bot.Builder;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace RestroQnABot.Utlities
{
    public class KnowleadgeBaseSettings
    {
        public List<string> KnowleageBaseSource;

        private IStatePropertyAccessor<KnowleadgeBaseSettings> _knowleadgeBaseAccessor;

        private UserState _userState;
        public KnowleadgeBaseSettings()
        {
            KnowleageBaseSource = new List<string>();
        }
        public KnowleadgeBaseSettings(UserState userState)
        {
            _userState = userState;
            _knowleadgeBaseAccessor = userState.CreateProperty<KnowleadgeBaseSettings>(nameof(KnowleadgeBaseSettings));
        }

        public void LoadKnowleadgeSources(KnowleadgeSourceData data)
        {
            
                foreach (var source in data.data)
                {

                    if (!string.IsNullOrEmpty(source))
                    {
                        KnowleageBaseSource.Add(source);
                    }
                }
        }

        public async Task saveKnowleageBaseSources(ITurnContext turnContext, KnowleadgeSourceData data, CancellationToken cancellationToken)
        {

            var knowleadgeBaseSettings = await _knowleadgeBaseAccessor.GetAsync(turnContext, () => new KnowleadgeBaseSettings(), cancellationToken);

            if (knowleadgeBaseSettings.KnowleageBaseSource.Count == 0) {

                knowleadgeBaseSettings.LoadKnowleadgeSources(data);
            }

            await _knowleadgeBaseAccessor.SetAsync(turnContext, knowleadgeBaseSettings, cancellationToken);
        }

        public async Task<KnowleadgeBaseSettings> getKnowleadgeBaseSources(ITurnContext turnContext, CancellationToken cancellationToken)
        {

            var knowleadgeBaseSettings = await _knowleadgeBaseAccessor.GetAsync(turnContext, () => new KnowleadgeBaseSettings(), cancellationToken);

            return knowleadgeBaseSettings;
        }

    }

    public class KnowleadgeSourceData
    {
        public string[] data { get; set; }
    }

}
