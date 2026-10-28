using System.Threading.Tasks;

namespace MuzeGecesi.Argus
{
    // Sahne ARGUS'a sadece bu arayüz üzerinden ulaşır
    public interface IArgusClient
    {
        Task<ArgusResponse> AskAsync(string playerText);

        // Modeli önceden belleğe yükler
        Task WarmUpAsync();

        // Yeni oyunda konuşma geçmişini siler
        void ResetConversation();
    }
}
