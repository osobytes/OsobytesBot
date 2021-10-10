using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using System;
using System.Threading.Tasks;

namespace UnityBotService.Credentials
{
    public class AzureSecretClient : ISecretClient
    {
        private readonly Lazy<SecretClient> Client;
        private const string KeyvaultUriKey = "OSOBYTES_KEYVAULT_URI";
        public AzureSecretClient()
        {
            Client = new Lazy<SecretClient>(() =>
            {
                var keyvaultUri = Environment.GetEnvironmentVariable(KeyvaultUriKey);
                if (string.IsNullOrWhiteSpace(keyvaultUri))
                {
                    throw new Exception("Could not find Keyvault uri environment variable.");
                }
                return new SecretClient(vaultUri: new Uri(keyvaultUri), credential: new DefaultAzureCredential());
            });
        }
        public async Task<string> GetSecretAsync(string key)
        {
            var result = await Client.Value.GetSecretAsync(key);
            if (string.IsNullOrWhiteSpace(result?.Value?.Value))
            {
                throw new Exception($"Could not fetch proper secret for given key {key}");
            }
            return result.Value.Value;
        }
    }
}
