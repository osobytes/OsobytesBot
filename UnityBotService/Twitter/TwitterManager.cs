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
                    var message = GetRandomReleaseMessage(release.Version);
                    if(message.Length > 140)
                    {
                        throw new Exception("Tweet is longer than 40 characters.");
                    }
                    var tweet = await Twitter.PublishTweetAsync(message);
                    Logger.LogInformation($"I just tweeted about Unity releasing version {release.Version}");
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
                var yearsStr = yearDiff == 1 ? "year" : "years";
                var unityV = release.UnityVersion.Replace("version-", "");
                var tweetText = $"A day like today {yearDiff} {yearsStr} ago, version {release.Version} was released for Unity {unityV}";
                var alreadyTweeted = await Twitter.IncludedInRecentTweetsAsync(tweetText);
                if (!alreadyTweeted)
                {
                    var tweet = await Twitter.PublishTweetAsync(tweetText);
                    Logger.LogInformation("I just tweeted an anniversary release.");
                }
            }
            catch (Exception ex)
            {
                var error = ex.ToString();
                Logger.LogError("tweet_send_error: {error}", error);
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

        private string GetRandomReleaseMessage(string version)
        {
            var random = new Random();
            var messageIndex = random.Next(0, Messages.Length - 1);
            var msg = Messages[messageIndex];
            var formattedMessage = string.Format(msg, version);
            formattedMessage = $"{formattedMessage} #Unity3d";
            return formattedMessage;
        }

        public static string[] Messages = new string[]
        {
            "The new Unity version {0} just came out of the oven! 👨‍🍳",
            "Stop the press everyone! 📰 Unity has just released version v{0}",
            "Hey, you might want to leave that for later and go get the new Unity release v{0}",
            "Remember how hard you worked to get that Unity game working? Well guess what, v{0} just arrived 😀",
            "Wow, yet another new Unity release. Go get v{0} now! 🚀",
            "Unity has just released version v{0}, what are you waiting for? 🎈",
            "Hey you wanna hear some good news?, Unity has just released version v{0}! ❤",
        };
    }
}