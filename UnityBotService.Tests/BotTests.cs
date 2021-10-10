using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Tweetinvi.Models;
using UnityBotService;
using UnityBotService.Credentials;
using UnityBotService.Twitter;
using UnityBotService.Unity;
using Xunit;

namespace UnityUpdateTweetBot.Tests
{
    public class BotTests
    {
        [Fact]
        public void ArchiveGetsLatestRelease()
        {
            var mockReleases = GetMockArchiverData();
            var unityArchive = new UnityArchive(mockReleases);
            var latestRelease = unityArchive.GetLatestRelease();
            Assert.NotNull(latestRelease);
            Assert.True(latestRelease?.ReleaseDate == new DateTime(2021, 9, 15));
        }

        [Fact]
        public async void ArchiveSuccesfullyParses()
        {
            var archiveHtml = GetArchiveHtml();
            var archiveParser = new UnityArchiveParser();
            var unityArchive = await archiveParser.ParseAsync(archiveHtml);
            Assert.NotEmpty(unityArchive.Releases);
        }

        [Fact]
        public async void CredentialManagerTest_ShouldFetchAzureKeyvaultAndReturnCredentials()
        {
            var secretClient = new Mock<ISecretClient>();
            var logger = new Mock<ILogger<CredentialManager>>();
            secretClient.Setup(s => s.GetSecretAsync(It.Is<string>(secretId => secretId == CredentialManager.TwitterBearerTokenName))).Returns(() => Task.FromResult("bearer"));
            secretClient.Setup(s => s.GetSecretAsync(It.Is<string>(secretId => secretId == CredentialManager.TwitterConsumerKeyName))).Returns(() => Task.FromResult("key"));
            secretClient.Setup(s => s.GetSecretAsync(It.Is<string>(secretId => secretId == CredentialManager.TwitterConsumerSecretName))).Returns(() => Task.FromResult("secret"));
            secretClient.Setup(s => s.GetSecretAsync(It.Is<string>(secretId => secretId == CredentialManager.TwitterAccessTokenName))).Returns(() => Task.FromResult("accesstoken"));
            secretClient.Setup(s => s.GetSecretAsync(It.Is<string>(secretId => secretId == CredentialManager.TwitterAccessSecretName))).Returns(() => Task.FromResult("accesstokensecret"));
            var secretM = new CredentialManager(logger.Object, secretClient.Object);
            var creds = await secretM.GetTwitterCredentialsAsync();
            Assert.NotNull(creds);
            Assert.Equal("key", creds.ConsumerKey);
            Assert.Equal("secret", creds.ConsumerSecret);
            Assert.Equal("bearer", creds.BearerToken);
            Assert.Equal("accesstoken", creds.AccessToken);
            Assert.Equal("accesstokensecret", creds.AccessTokenSecret);
        }

        [Fact]
        public async void CredentialManagerTest_ShouldThrowWhenCredentialIsNullOrEmpty()
        {
            var secretClient = new Mock<ISecretClient>();
            var logger = new Mock<ILogger<CredentialManager>>();
            
            secretClient.Setup(s => s.GetSecretAsync(It.Is<string>(secretId => secretId == CredentialManager.TwitterBearerTokenName))).Returns(async () => await Task.FromResult(""));
            secretClient.Setup(s => s.GetSecretAsync(It.Is<string>(secretId => secretId == CredentialManager.TwitterConsumerKeyName))).Returns(async () => await Task.FromResult(""));
            secretClient.Setup(s => s.GetSecretAsync(It.Is<string>(secretId => secretId == CredentialManager.TwitterConsumerSecretName))).Returns(async () => await Task.FromResult(""));
            secretClient.Setup(s => s.GetSecretAsync(It.Is<string>(secretId => secretId == CredentialManager.TwitterAccessTokenName))).Returns(async () => await Task.FromResult(""));
            secretClient.Setup(s => s.GetSecretAsync(It.Is<string>(secretId => secretId == CredentialManager.TwitterAccessSecretName))).Returns(async () => await Task.FromResult(""));
            var secretM = new CredentialManager(logger.Object, secretClient.Object);
            await Assert.ThrowsAsync<Exception>(async () => await secretM.GetTwitterCredentialsAsync());
        }

        [Fact]
        public async void BotTests_ShouldTweetIfUnityReleasedToday()
        {
            var hostLife = new Mock<IHostApplicationLifetime>();
            var logger = new Mock<ILogger<Bot>>();
            var twitter = new Mock<ITwitterManager>();
            var archiveReader = new Mock<IUnityArchiveReader>();
            var archive = new Mock<UnityArchive>();
            var release = new Mock<UnityRelease>();

            release.Setup(r => r.ReleaseDate).Returns(() => DateTime.Today);
            archive.Setup(a => a.GetLatestRelease()).Returns(() => release.Object);
            archiveReader.Setup(a => a.ReadLatestArchiveAsync()).Returns(async () => await Task.FromResult(archive.Object));
            twitter.Setup(t => t.TweetNewUnityReleaseAsync(It.Is<UnityRelease>(r => r == release.Object))).Returns(async () => await Task.CompletedTask);

            var bot = new Bot(logger.Object, hostLife.Object, twitter.Object, archiveReader.Object);

            await bot.TweetIfUnityReleasedTodayAsync();

            twitter.Verify(t => t.TweetNewUnityReleaseAsync(It.IsAny<UnityRelease>()), Times.Once());
        }

        [Fact]
        public async void BotTests_TwitterApiInitializesCredentials()
        {
            var twitterClient = new Mock<ITwitterClient>();
            var logger = new Mock<ILogger<TwitterManager>>();
            var twitter = new TwitterManager(logger.Object, twitterClient.Object);
            var newTweet = new Mock<ITweet>();
            
            var release = new UnityRelease()
            {
                ReleaseDate = DateTime.Today,
                UnityVersion = "11",
                Version = "12.10",
            };
            twitterClient.Setup(c => c.PublishTweetAsync(It.IsAny<string>())).Returns(async () => await Task.FromResult("newtweetid"));

            await twitter.TweetNewUnityReleaseAsync(release);
            twitterClient.Verify(c => c.PublishTweetAsync(It.IsAny<string>()), Times.Once());
        }

        [Fact]
        public async void BotTests_TwitterApiShouldNotPublishTweetIfAlreadyTweeted()
        {
            var client = new Mock<ITwitterClient>();
            var logger = new Mock<ILogger<TwitterManager>>();
            var twitter = new TwitterManager(logger.Object, client.Object);
            var timelineTweet = new Mock<ITweet>();
            timelineTweet.Setup(t => t.Text).Returns(() => "Unity version 12.10 has been released. Would you like to update?");
            timelineTweet.Setup(t => t.CreatedAt).Returns(() => DateTime.Today);

            var release = new UnityRelease()
            {
                ReleaseDate = DateTime.Today,
                UnityVersion = "11",
                Version = "12.10",
            };
            client.Setup(c => c.IncludedInRecentTweetsAsync(It.IsAny<string>())).Returns(async () => await Task.FromResult(true));
            await twitter.TweetNewUnityReleaseAsync(release);
            client.Verify(c => c.PublishTweetAsync(It.IsAny<string>()), Times.Never());
        }

        private static string GetArchiveHtml()
        {
            var binPath = Assembly.GetExecutingAssembly().Location;
            var path = Path.Combine(Path.GetDirectoryName(binPath)!, "archive.html");
            Assert.True(File.Exists(path));
            return File.ReadAllText(path);
        }

        private static IDictionary<string, IReadOnlyList<UnityRelease>> GetMockArchiverData()
        {
            var binPath = Assembly.GetExecutingAssembly().Location;
            var path = Path.Combine(Path.GetDirectoryName(binPath)!, "mockReleases.json");
            Assert.True(File.Exists(path));
            var text = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<IDictionary<string, IReadOnlyList<UnityRelease>>>(text)!;
        }
    }
}