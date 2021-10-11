using Moq;
using System.Threading.Tasks;
using UnityBotService.Credentials;
using UnityBotService.Twitter;
using Xunit;

namespace UnityBotService.Tests
{
    public class TwitterApiTests
    {
        // NOTE: You should not push any credentials to your own repository.
        // This tests are disabled and these are used to test out new features or things that will interact
        // directly with the twitter api.
        // This is a template that can be used to do local testing.
        public const string ConsumerKey = "<replace with consumer key>";
        public const string ConsumerSecret = "<replace with consumer secret>";
        public const string AccessToken = "<replace with access token>";
        public const string AccessTokenSecret = "<replace with access token secret>";
        public const string BearerToken = "<replace with bearer token>";
        public bool TestsEnabled = false;

        [Fact]
        public async void BotTest_TryTweet()
        {
            if (!TestsEnabled)
            {
                return;
            }

            var credentialManager = new Mock<ICredentialManager>();
            var twitterCreds = GetCredentials();

            credentialManager.Setup(c => c.GetTwitterCredentialsAsync()).Returns(() => Task.FromResult(twitterCreds));
            var client = new TweetInviClient(credentialManager.Object);

            await client.PublishTweetAsync("Test tweet.");
        }

        [Fact]
        public async void BotTest_TryGetScreenName()
        {
            if (!TestsEnabled)
            {
                return;
            }

            var credentialManager = new Mock<ICredentialManager>();
            var twitterCreds = GetCredentials();

            credentialManager.Setup(c => c.GetTwitterCredentialsAsync()).Returns(() => Task.FromResult(twitterCreds));
            var client = new TweetInviClient(credentialManager.Object);

            var screenName = await client.GetConnectedUserScreenNameAsync();
            Assert.NotNull(screenName);
        }

        [Fact]
        public async void BotTest_TryGetIncludedInRecentTweetsAsync()
        {
            if (!TestsEnabled)
            {
                return;
            }

            var credentialManager = new Mock<ICredentialManager>();
            var twitterCreds = GetCredentials();

            credentialManager.Setup(c => c.GetTwitterCredentialsAsync()).Returns(() => Task.FromResult(twitterCreds));
            var client = new TweetInviClient(credentialManager.Object);

            var isIncluded = await client.IncludedInRecentTweetsAsync("test tweet.");
        }

        private static TwitterCredentials GetCredentials()
        {
            return new TwitterCredentials(
                consumerKey: ConsumerKey,
                consumerSecret: ConsumerSecret,
                accessToken: AccessToken,
                accessTokenSecret: AccessTokenSecret,
                bearerToken: BearerToken
                );
        }
    }
}
