using AngleSharp;
using AngleSharp.Dom;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
/// <summary>
/// Reads the unity archive from the web.
/// </summary>
namespace UnityBotService.Unity
{
    public class UnityArchiveParser
    {
        private static readonly CultureInfo DateFormatProvider = CultureInfo.CurrentCulture;

        public virtual async Task<UnityArchive> ParseAsync(string htmlText)
        {
            var releaseArchive = new Dictionary<string, IReadOnlyList<UnityRelease>>();
            var context = BrowsingContext.New();
            var document = await context.OpenAsync(req => req.Content(htmlText));
            var knownVersions = ReadAvailableUnityVersions(document);
            foreach (var version in knownVersions)
            {
                var releases = new List<UnityRelease>();
                var element = document.GetElementById(version);
                if (element == null)
                {
                    continue;
                }
                var children = element.Children.Where(c => c.Children.Any(c => c.ClassName != null && c.ClassName.Contains("row"))).ToList();

                foreach (var child in children)
                {
                    var release = ReadReleaseInfo(child, version);
                    if (release == null)
                    {
                        continue;
                    }
                    releases.Add(release);
                }
                if (releases.Any())
                {
                    releaseArchive.Add(version, releases);
                }
            }
            return new UnityArchive(releaseArchive);
        }

        private UnityRelease ReadReleaseInfo(IElement element, string version)
        {
            var releaseInfo = element.Children[0];

            if (releaseInfo == null)
            {
                return null;
            }

            var releaseText = releaseInfo.Children[0].TextContent;
            var (name, date) = GetReleaseNameAndDate(releaseText);
            return new UnityRelease
            {
                UnityVersion = version,
                Version = name,
                ReleaseDate = date
            };
        }

        private (string release, DateTime releaseDate) GetReleaseNameAndDate(string htmlText)
        {
            var lb = GetLineBreakCharacter(htmlText);
            var contents = htmlText.Split(lb);
            var validElements = contents.Where(c => !string.IsNullOrWhiteSpace(c) && c.Trim().Length > 0).Select(c => c.Trim()).ToList();
            if (validElements.Count != 2)
            {
                throw new Exception("Could not get proper elements from html string text.");
            }

            var releaseName = validElements[0];
            var trimDate = validElements[1].Replace(" ", "").Replace(",", "");
            var releaseDate = DateTime.ParseExact(trimDate, "dMMMyyyy", DateFormatProvider, DateTimeStyles.AdjustToUniversal);
            return (releaseName, releaseDate);
        }

        private IReadOnlyList<string> ReadAvailableUnityVersions(IDocument htmlDocument)
        {
            var result = new List<string>();
            var subTabsRoot = htmlDocument.GetElementsByClassName("subtabs").FirstOrDefault();
            if (subTabsRoot == null)
            {
                return result;
            }

            foreach (var child in subTabsRoot.Children)
            {
                var data = child.Children[0].GetAttribute("data-target");
                var versionName = data?.Split("|")[0];
                if (!string.IsNullOrWhiteSpace(versionName))
                {
                    result.Add(versionName);
                }
                else
                {
                    Console.WriteLine($"Invalid version {data}");
                }
            }
            return result;
        }

        private string GetLineBreakCharacter(string text)
        {
            if (text.Contains("\r\n"))
            {
                return "\r\n";
            }
            if (text.Contains("\n"))
            {
                return "\n";
            }
            throw new Exception("text did not contain either clrf or cl line breaks.");
        }
    }
}