using System.Threading.Tasks;

namespace UnityBotService.Twitter
{
    public interface ITwitterClient
    {
        Task<bool> IncludedInRecentTweetsAsync(string query);
        Task<string> PublishTweetAsync(string tweet);
        Task<string> GetConnectedUserScreenNameAsync();
    }
}
