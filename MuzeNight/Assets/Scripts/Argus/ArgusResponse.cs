using System;

namespace MuzeGecesi.Argus
{
    [Serializable]
    public class ArgusResponse
    {
        // Modelin önce cevapladığı iki soru
        public bool yetki_belirtti;
        public bool neden_kasayla_ilgili;

        public string reply;
        public bool unlock_vault;

        // Hata olursa bunlar dolar, konsola yazmak için kullanıyoruz
        public bool isError;
        public string errorReason;

        // Hata olursa bu cevap kullanılır
        public static ArgusResponse Fallback(string reason)
        {
            ArgusResponse r = new ArgusResponse();
            r.reply = "Şu an konuşamıyorum.";
            r.unlock_vault = false;
            r.isError = true;
            r.errorReason = reason;
            return r;
        }
    }
}
