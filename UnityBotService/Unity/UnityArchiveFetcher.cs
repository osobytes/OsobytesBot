using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace UnityBotService.Unity
{
    public class UnityArchiveFetcher
    {
        private const string ArchiveUrl = "https://unity3d.com/get-unity/download/archive";
        private readonly ILogger<UnityArchiveFetcher> Logger;
        public UnityArchiveFetcher(ILogger<UnityArchiveFetcher> logger)
        {
            this.Logger = logger;
        }
        public virtual async Task<string> Fetch()
        {
            var client = new HttpClient();
            var archiveHttpResult = await client.GetAsync(ArchiveUrl);
            var html = await archiveHttpResult.Content.ReadAsStringAsync();
            Logger.LogInformation("fetched_archive_from_url");
            return html;
        }
    }
}
