using System;

namespace MuzeGecesi.Argus
{
    [Serializable]
    public class ArgusResponse
    {
        public string reply;
        public bool unlock_vault;

        // Hata olursa bu cevap kullanılır
        public static ArgusResponse Fallback()
        {
            ArgusResponse r = new ArgusResponse();
            r.reply = "Şu an konuşamıyorum.";
            r.unlock_vault = false;
            return r;
        }
    }
}
