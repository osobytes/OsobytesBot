using System.IO;
using System.Threading.Tasks;

namespace UnityBotService.Credentials
{
    public class MountSecretClient : ISecretClient
    {
        private const string MountPath = "/mnt/secrets";
        public async Task<string> GetSecretAsync(string key)
        {
            var path = Path.Combine(MountPath, key);
            if (!File.Exists(path))
            {
                return null;
            }
            var secretValue = await File.ReadAllTextAsync(path);
            return secretValue.Trim();
        }
    }
}
