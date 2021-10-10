using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using UnityBotService.Twitter;
using UnityBotService.Unity;

namespace UnityBotService
{
    public class Bot : BackgroundService
    {
        private static TimeSpan IntervalDelay = TimeSpan.FromMinutes(60);
        private ITwitterManager Twitter { get; set; }
        private IUnityArchiveReader Archive { get; set; }
        private ILogger<Bot> Logger { get; set; }
        private IHostApplicationLifetime Application { get; set; }
        public Bot(ILogger<Bot> logger, IHostApplicationLifetime app, ITwitterManager twitter, IUnityArchiveReader archive)
        {
            this.Twitter = twitter;
            this.Archive = archive;
            this.Logger = logger;
            this.Application = app;
        }

        public async Task TweetIfUnityReleasedTodayAsync()
        {
            try
            {
                var archive = await Archive.ReadLatestArchiveAsync();

                if (archive == null)
                {
                    throw new ArgumentNullException("archive", "Archive fetch returned null");
                }

                var latest = archive.GetLatestRelease();
                var today = DateTime.Now.Date;
                if (today == latest?.ReleaseDate.Date)
                {
                    await Twitter.TweetNewUnityReleaseAsync(latest);
                }
            }catch(Exception ex)
            {
                Logger.LogError("tweet_if_unity_released_today_error", ex.ToString());
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                await Twitter.VerifyConnection();
            }
            catch(Exception ex)
            {
                Logger.LogError($"Could not verify twitter connection: {ex}");
                return;
            }
            
            while (!stoppingToken.IsCancellationRequested)
            {
                Logger.LogInformation("Unity Bot run archive check: {time}", DateTimeOffset.Now);
                await TweetIfUnityReleasedTodayAsync();
                await Task.Delay(IntervalDelay, stoppingToken);
            }
        }
    }
}
