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
                    Logger.LogInformation("I just tweeted a brand new version release.");
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"tweet_send_error", ex.ToString());
            }
        }

        public async Task TweetUnityReleaseAnniversary(UnityRelease release, int yearDiff)
        {
            try
            {
                var tweetText = $"A day like today {yearDiff} years ago, Unity released version {release.Version} for Unity {release.UnityVersion}";
                var alreadyTweeted = await Twitter.IncludedInRecentTweetsAsync(tweetText);
                if (!alreadyTweeted)
                {
                    var tweet = await Twitter.PublishTweetAsync(tweetText);
                    Logger.LogInformation("I just tweeted an anniversary release.");
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"tweet_send_error", ex.ToString());
            }
        }

        public async Task VerifyConnection()
        {
            var user = await Twitter.GetConnectedUserScreenNameAsync();
            if (string.IsNullOrWhiteSpace(user))
            {
                throw new Exception("Could not fetch connected user from twitter client.");
            }
            Logger.LogInformation($"Connected to twitter with user account: {user}");
        }
    }
}