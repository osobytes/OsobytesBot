using System.Threading.Tasks;
using Tweetinvi;
using Tweetinvi.Models;
using UnityBotService.Credentials;

namespace UnityBotService.Twitter
{
    public class TweetInviClient : ITwitterClient
    {
        private TwitterClient Client;
        private IAuthenticatedUser BotAccount;
        private readonly ICredentialManager CredentialManager;
        public TweetInviClient(ICredentialManager credentials)
        {
            CredentialManager = credentials;
        }

        private async Task InitializeClientAsync()
        {
            var twitterCredentials = await CredentialManager.GetTwitterCredentialsAsync();
            Client = new TwitterClient(twitterCredentials.ConsumerKey, twitterCredentials.ConsumerSecret, twitterCredentials.AccessToken, twitterCredentials.AccessTokenSecret);
            BotAccount = await Client!.Users.GetAuthenticatedUserAsync();
        }

        private async Task<IAuthenticatedUser> GetAccount()
        {
            if (Client == null || BotAccount == null)
            {
                await InitializeClientAsync();
                BotAccount = await Client!.Users.GetAuthenticatedUserAsync();
            }
            return BotAccount;
        }

        public async Task<ITweet[]> GetUserTimelineAsync()
        {
            var account = await GetAccount();
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
            var account = await GetAccount();
            var tweetItem = await account.Client.Tweets.PublishTweetAsync(tweet);
            return tweetItem.IdStr;
        }

        public async Task<string> GetConnectedUserNameAsync()
        {
            var user = await GetAccount();
            return user.Name;
        }
    }
}
