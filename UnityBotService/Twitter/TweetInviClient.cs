using System.Threading.Tasks;
using Tweetinvi;
using Tweetinvi.Models;
using UnityBotService.Credentials;

namespace UnityBotService.Twitter
{
    public class TweetInviClient : ITwitterClient
    {
        private TwitterClient TwitterClient;
        private readonly ICredentialManager CredentialManager;
        public TweetInviClient(ICredentialManager credentials)
        {
            CredentialManager = credentials;
        }

        private async Task<TwitterClient> GetTwitterClientAsync()
        {
            if(TwitterClient == null)
            {
                var credentials = await CredentialManager.GetTwitterCredentialsAsync();
                TwitterClient = new TwitterClient(credentials);
            }

            return TwitterClient;
        }

        private async Task<IAuthenticatedUser> GetUserAsync()
        {
            var client = await GetTwitterClientAsync();
            return await client.Users.GetAuthenticatedUserAsync();
        }

        public async Task<ITweet[]> GetUserTimelineAsync()
        {
            var account = await GetUserAsync();
            return await account.GetUserTimelineAsync();
        }

        public async Task<bool> IncludedInRecentTweetsAsync(string query)
        {
            var timeline = await GetUserTimelineAsync();
            foreach(var tweet in timeline)
            {
                if (tweet.Text.Contains(query))
                {
                    return true;
                }
            }
            return false;
        }

        public async Task<string> PublishTweetAsync(string tweet)
        {
            var client = await GetTwitterClientAsync();
            var tweetItem = await client.Tweets.PublishTweetAsync(tweet);
            return tweetItem.IdStr;
        }

        public async Task<string> GetConnectedUserScreenNameAsync()
        {
            var user = await GetUserAsync();
            return user.ScreenName;
        }
    }
}