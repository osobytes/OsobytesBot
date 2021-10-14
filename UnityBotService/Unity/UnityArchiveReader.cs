using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace UnityBotService.Unity
{
    public class UnityArchiveReader : IUnityArchiveReader
    {
        private readonly UnityArchiveParser Parser;
        private readonly UnityArchiveFetcher ArchiveFetcher;
        private readonly ILogger<UnityArchiveReader> Logger;
        public UnityArchiveReader(UnityArchiveParser parser, UnityArchiveFetcher fetcher, ILogger<UnityArchiveReader> logger)
        {
            this.Parser = parser;
            this.ArchiveFetcher = fetcher;
            this.Logger = logger;
        }

        public async Task<UnityArchive> ReadLatestArchiveAsync()
        {
            try
            {
                var htmlData = await ArchiveFetcher.Fetch();
                var parsedData = await Parser.ParseAsync(htmlData);
                return parsedData;
            }
            catch(Exception ex)
            {
                Logger.LogError("read_latest_archive", ex);
            }
            return null;
        }
    }
}
