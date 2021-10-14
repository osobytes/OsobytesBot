using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Threading.Tasks;
using Tweetinvi.Models;
using UnityBotService.Twitter;
using UnityBotService.Unity;
using Xunit;

namespace UnityUpdateTweetBot.Tests
{
    public class TwitterManagerTests
    {
        [Fact]
        public async void TwitterManagerTests_ItPublishesTweet()
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
        public async void TwitterManagerTests_ShouldNotPublishTweetIfAlreadyTweeted()
        {
            var client = new Mock<ITwitterClient>();
            var logger = new Mock<ILogger<TwitterManager>>();
            var twitter = new TwitterManager(logger.Object, client.Object);
            var timelineTweet = new Mock<ITweet>();
            timelineTweet.Setup(t => t.Text).Returns(() => "Unity version 12.10 has been released.");
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

        [Fact]
        public async void TwitterManagerTests_MessagesShouldNotExceed140Chars()
        {
            var maxVersionlEngth = 10;

            foreach(var message in TwitterManager.Messages)
            {
                var r = message.Replace("{0}", "");
                r = $"{r} #Unity3d";
                Assert.True(r.Length + maxVersionlEngth <= 140);
            }
        }
    }
}