using System.Threading.Tasks;

namespace UnityBotService.Unity
{
    public interface IUnityArchiveReader
    {
        Task<UnityArchive> ReadLatestArchiveAsync();
    }
}
