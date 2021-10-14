namespace UnityBotService.Credentials
{
    using System.Threading.Tasks;
    public interface ISecretClient
    {
        Task<string> GetSecretAsync(string key);
    }
}
