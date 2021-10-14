using Tweetinvi.Models;

namespace UnityBotService.Twitter
{
    public class TwitterCredentials : IReadOnlyTwitterCredentials
    {
        public TwitterCredentials(string consumerKey, string consumerSecret, string accessToken, string accessTokenSecret, string bearerToken)
        {
            BearerToken = bearerToken;
            ConsumerKey = consumerKey;
            ConsumerSecret = consumerSecret;
            AccessTokenSecret = accessTokenSecret;
            AccessToken = accessToken;
        }

        public string ConsumerKey { get; private set; }
        public string ConsumerSecret { get; private set; }
        public string AccessToken { get; set; }
        public string AccessTokenSecret { get; set; }
        public string BearerToken { get; private set; }
    }
}
