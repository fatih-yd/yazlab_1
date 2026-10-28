using System.Threading;
using System.Threading.Tasks;

namespace MuzeGecesi.Argus
{
    // Sahne ARGUS'a sadece bu arayüz üzerinden ulaşır
    public interface IArgusClient
    {
        Task<ArgusResponse> AskAsync(string playerText, CancellationToken token = default);
    }
}
