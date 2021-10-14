using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityBotService;
using UnityBotService.Twitter;
using UnityBotService.Unity;
using Xunit;

namespace UnityUpdateTweetBot.Tests
{
    public class BotTests
    {
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
            twitter.Setup(t => t.TweetNewUnityReleaseAsync(It.Is<UnityRelease>(r => r == release.Object))).Returns(async () => await Task.CompletedTask);

            var bot = new Bot(logger.Object, hostLife.Object, twitter.Object, archiveReader.Object);

            await bot.TweetIfUnityReleasedTodayAsync(archive.Object);

            twitter.Verify(t => t.TweetNewUnityReleaseAsync(It.IsAny<UnityRelease>()), Times.Once());
        }

        [Fact]
        public async void BotTests_ShouldTweetIfUnityAnniversaryIsToday()
        {
            var hostLife = new Mock<IHostApplicationLifetime>();
            var logger = new Mock<ILogger<Bot>>();
            var twitter = new Mock<ITwitterManager>();
            var archiveReader = new Mock<IUnityArchiveReader>();
            var archive = new Mock<UnityArchive>();
            var releasedToday = new Mock<UnityRelease>();
            releasedToday.Setup(r => r.ReleaseDate).Returns(() => DateTime.Now.Date);
            var release = new Mock<UnityRelease>();
            release.Setup(r => r.ReleaseDate).Returns(() => DateTime.Now.Date.AddYears(-1));
            release.Setup(r => r.UnityVersion).Returns(() => "v1");
            release.Setup(r => r.Version).Returns(() => "v11");
            var release2 = new Mock<UnityRelease>();
            release2.Setup(r => r.ReleaseDate).Returns(() => DateTime.Now.Date.AddYears(-3));
            release2.Setup(r => r.UnityVersion).Returns(() => "v2");
            release2.Setup(r => r.Version).Returns(() => "v21");
            var dictionary = new Dictionary<string, IReadOnlyList<UnityRelease>>();
            dictionary.Add("v0", new UnityRelease[] { releasedToday.Object });
            dictionary.Add("v1", new UnityRelease[] { release.Object });
            dictionary.Add("v2", new UnityRelease[] { release2.Object });
            archive.Setup(a => a.UnityVersions).Returns(() => new string[] { "v0", "v1", "v2" });
            archive.Setup(a => a.Releases).Returns(() => dictionary);

            twitter.Setup(t => t.TweetUnityReleaseAnniversary(It.Is<UnityRelease>(r => r == release.Object), It.Is<int>(yd => yd == 1))).Returns(async () => await Task.CompletedTask);
            twitter.Setup(t => t.TweetUnityReleaseAnniversary(It.Is<UnityRelease>(r => r == release2.Object), It.Is<int>(yd => yd == 3))).Returns(async () => await Task.CompletedTask);
            var bot = new Bot(logger.Object, hostLife.Object, twitter.Object, archiveReader.Object);

            await bot.TweetIfUnityReleaseAniversary(archive.Object);

            twitter.Verify(t => t.TweetUnityReleaseAnniversary(It.Is<UnityRelease>(r => r == release.Object), It.Is<int>(yd => yd == 1)), Times.Once());
            twitter.Verify(t => t.TweetUnityReleaseAnniversary(It.Is<UnityRelease>(r => r == release2.Object), It.Is<int>(yd => yd == 3)), Times.Once());
            twitter.Verify(t => t.TweetUnityReleaseAnniversary(It.IsAny<UnityRelease>(), It.IsAny<int>()), Times.Exactly(2));
        }
    }
}