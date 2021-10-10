using System.Threading.Tasks;
using UnityBotService.Twitter;

namespace UnityBotService.Credentials
{
    public interface ICredentialManager
    {
        public Task<TwitterCredentials> GetTwitterCredentialsAsync();
    }
}
