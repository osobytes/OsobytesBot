using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityBotService.Unity;
using Xunit;

namespace UnityUpdateTweetBot.Tests
{
    public class UnityArchiveTests
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