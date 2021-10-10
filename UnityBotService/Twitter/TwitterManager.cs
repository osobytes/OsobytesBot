using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using UnityBotService.Unity;

namespace UnityBotService.Twitter
{
    public class TwitterManager : ITwitterManager
    {
        private readonly ITwitterClient Twitter;
        private readonly ILogger<TwitterManager> Logger;
        public TwitterManager(ILogger<TwitterManager> logger, ITwitterClient twitterClient)
        {
            this.Logger = logger;
            this.Twitter = twitterClient;
        }

        public async Task TweetNewUnityReleaseAsync(UnityRelease release)
        {
            try
            {
                var alreadyTweeted = await Twitter.IncludedInRecentTweetsAsync(release.Version);
                if (!alreadyTweeted)
                {
                    var tweet = await Twitter.PublishTweetAsync($"A new version of Unity ({release.Version}) is Available.");
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"tweet_send_error", ex.ToString());
            }
        }

        public async Task VerifyConnection()
        {
            var user = await Twitter.GetConnectedUserNameAsync();
            if (string.IsNullOrWhiteSpace(user))
            {
                throw new Exception("Could not fetch connected user from twitter client.");
            }
            Logger.LogInformation($"Connected to twitter with user account: {user}");
        }
    }
}