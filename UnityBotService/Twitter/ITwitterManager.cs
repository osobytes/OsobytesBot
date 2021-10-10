using System.Threading.Tasks;
using UnityBotService.Unity;

namespace UnityBotService.Twitter
{
    public interface ITwitterManager
    {
        Task TweetNewUnityReleaseAsync(UnityRelease release);
        Task TweetUnityReleaseAnniversary(UnityRelease release, int yearDiff);
        Task VerifyConnection();
    }
}
