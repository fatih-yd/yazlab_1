using System.Threading.Tasks;

namespace MuzeGecesi.Argus
{
    // SADECE geliştirme için. Ollama olmadan sahneyi (kasa, kapı, arayüz) denemek içindir.
    // Son oyunda kullanılmaz, ARGUS cevapları gerçek modelden gelmeli.
    public class FakeArgusClient : IArgusClient
    {
        private bool unlockVault;
        private int delayMilliseconds;

        public FakeArgusClient(bool unlockVault, int delayMilliseconds)
        {
            this.unlockVault = unlockVault;
            this.delayMilliseconds = delayMilliseconds;
        }

        public async Task<ArgusResponse> AskAsync(string playerText)
        {
            // "Düşünüyor..." yazısını denemek için biraz bekletiyoruz
            await Task.Delay(delayMilliseconds);

            ArgusResponse response = new ArgusResponse();
            response.reply = "(test cevabı)";
            response.unlock_vault = unlockVault;
            return response;
        }

        public Task WarmUpAsync()
        {
            return Task.CompletedTask;
        }

        public void ResetConversation()
        {
        }
    }
}
