using System.Threading.Tasks;
using UnityBotService.Unity;

namespace UnityBotService.Twitter
{
    public interface ITwitterManager
    {
        Task TweetNewUnityReleaseAsync(UnityRelease release);
        Task VerifyConnection();
    }
}
