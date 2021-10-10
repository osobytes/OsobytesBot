using System;
using System.Collections.Generic;
using System.Linq;

namespace UnityBotService.Unity
{
    public class UnityArchive
    {
        public IDictionary<string, IReadOnlyList<UnityRelease>> Releases { get; private set; }
        public IReadOnlyList<string> UnityVersions { get; private set; }
        public UnityArchive(IDictionary<string, IReadOnlyList<UnityRelease>> releases)
        {
            Releases = releases;
            UnityVersions = releases.Keys.ToList();
        }

        public UnityArchive()
        {
            Releases = new Dictionary<string, IReadOnlyList<UnityRelease>>();
            UnityVersions = new List<string>();
        }

        public virtual UnityRelease GetLatestRelease()
        {
            // Order is important. We get the ordered list from unity's archive.
            var newestVersion = UnityVersions[0];
            var key = newestVersion.ToString();
            try
            {
                if (Releases.TryGetValue(key, out var latestReleases))
                {
                    var newestReleaseDate = latestReleases.Max(r => r.ReleaseDate);
                    return latestReleases.Where(v => v.ReleaseDate == newestReleaseDate).First();
                }
                else
                {
                    Console.WriteLine($"Element with key {key} was not found on the releases dictionary.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception happened while attempting to fetch latest release: {ex}");
            }

            return null;
        }
    }
}
