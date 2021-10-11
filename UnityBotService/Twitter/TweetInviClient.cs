using System.Threading.Tasks;
using Tweetinvi;
using Tweetinvi.Models;
using UnityBotService.Credentials;

namespace UnityBotService.Twitter
{
    public class TweetInviClient : ITwitterClient
    {
        private IAuthenticatedUser BotAccount;
        private readonly ICredentialManager CredentialManager;
        private TwitterCredentials Credentials;
        public TweetInviClient(ICredentialManager credentials)
        {
            CredentialManager = credentials;
        }

        private async Task<TwitterCredentials> GetCredentials()
        {
            if(Credentials == null)
            {
                Credentials = await CredentialManager.GetTwitterCredentialsAsync();
            }
            return Credentials;
        }

        private async Task<IAuthenticatedUser> GetUserAsync()
        {
            if(BotAccount == null)
            {
                var credentials = await GetCredentials();
                var client = new TwitterClient(new InviCredentials(credentials));
                BotAccount = await client.Users.GetAuthenticatedUserAsync();
            }
            return BotAccount;
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
            var account = await GetUserAsync();
            var tweetItem = await account.Client.Tweets.PublishTweetAsync(tweet);
            return tweetItem.IdStr;
        }

        public async Task<string> GetConnectedUserScreenNameAsync()
        {
            var user = await GetUserAsync();
            return user.ScreenName;
        }
    }

    public class InviCredentials : IReadOnlyTwitterCredentials
    {
        public InviCredentials(TwitterCredentials credentials)
        {
            AccessToken = credentials.AccessToken;
            AccessTokenSecret = credentials.AccessTokenSecret;
            BearerToken = credentials.BearerToken;
            ConsumerKey = credentials.ConsumerKey;
            ConsumerSecret = credentials.ConsumerSecret;
        }
        public string AccessToken { get; private set; }

        public string AccessTokenSecret { get; private set; }

        public string BearerToken { get; private set; }

        public string ConsumerKey { get; private set; }

        public string ConsumerSecret { get; private set; }
    }
}
