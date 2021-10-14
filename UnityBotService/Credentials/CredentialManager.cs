using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using UnityBotService.Twitter;

namespace UnityBotService.Credentials
{
    public class CredentialManager : ICredentialManager
    {
        private readonly ISecretClient SecretClient;
        private readonly ILogger<CredentialManager> Logger;
        private const string UnityUpdateBotTwitterPrefix = "unityupdatebot-twitter-";
        public const string TwitterConsumerKeyName = UnityUpdateBotTwitterPrefix + "apikey";
        public const string TwitterConsumerSecretName = UnityUpdateBotTwitterPrefix + "apisecret";
        public const string TwitterBearerTokenName = UnityUpdateBotTwitterPrefix + "bearertoken";
        public const string TwitterAccessTokenName = UnityUpdateBotTwitterPrefix + "accesstoken";
        public const string TwitterAccessSecretName = UnityUpdateBotTwitterPrefix + "accesstokensecret";
        public CredentialManager(ILogger<CredentialManager> logger, ISecretClient secretClient)
        {
            this.SecretClient = secretClient;
            this.Logger = logger;
        }

        public async Task<TwitterCredentials> GetTwitterCredentialsAsync()
        {
            this.Logger.LogInformation("Reading credentials");
            var consumerKey = await this.SecretClient.GetSecretAsync(TwitterConsumerKeyName);
            var consumerSecret = await this.SecretClient.GetSecretAsync(TwitterConsumerSecretName);
            var accessToken = await this.SecretClient.GetSecretAsync(TwitterAccessTokenName);
            var accessTokenSecret = await this.SecretClient.GetSecretAsync(TwitterAccessSecretName);
            var bearerToken = await this.SecretClient.GetSecretAsync(TwitterBearerTokenName);
            var creds = new TwitterCredentials(consumerKey, consumerSecret, accessToken, accessTokenSecret, bearerToken);
            ValidateTwitterCredentials(creds);
            return creds;
        }

        private static void ValidateTwitterCredentials(TwitterCredentials creds)
        {
            if (string.IsNullOrWhiteSpace(creds.ConsumerKey))
            {
                throw new Exception("Twitter credentials consumer key cannot be empty.");
            }
            if (string.IsNullOrWhiteSpace(creds.ConsumerSecret))
            {
                throw new Exception("Twitter credentials consumer secret cannot be empty.");
            }
            if (string.IsNullOrWhiteSpace(creds.BearerToken))
            {
                throw new Exception("Twitter credentials bearer token cannot be empty.");
            }
            if (string.IsNullOrWhiteSpace(creds.AccessToken))
            {
                throw new Exception("Twitter credentials access token cannot be empty.");
            }
            if (string.IsNullOrWhiteSpace(creds.AccessTokenSecret))
            {
                throw new Exception("Twitter credentials access token secret cannot be empty.");
            }
        }
    }
}
