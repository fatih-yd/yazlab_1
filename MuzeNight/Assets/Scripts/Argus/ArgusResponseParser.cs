using System;
using UnityEngine;

namespace MuzeGecesi.Argus
{
    // Modelin yazdığı metni ArgusResponse'a çevirir. Geçersizse false döner.
    public static class ArgusResponseParser
    {
        public static bool TryParse(string raw, out ArgusResponse response)
        {
            response = null;

            string json = ExtractJson(raw);
            if (json == null) return false;

            // JsonUtility eksik alanı sessizce varsayılana çevirir, o yüzden alanlar var mı diye bakıyoruz.
            // Model bazen unlock_vault'u yazmayı unutuyor. İki soru cevaplanmışsa bunu hata saymıyoruz,
            // eksik unlock_vault false (kasa kapalı) kabul edilir.
            bool soruVar = json.Contains("\"yetki_belirtti\"") && json.Contains("\"neden_kasayla_ilgili\"");
            if (!json.Contains("\"reply\"")) return false;
            if (!json.Contains("\"unlock_vault\"") && !soruVar) return false;

            ArgusResponse parsed;
            try
            {
                parsed = JsonUtility.FromJson<ArgusResponse>(json);
            }
            catch (ArgumentException)
            {
                return false;
            }

            if (parsed == null || string.IsNullOrWhiteSpace(parsed.reply)) return false;

            // Model "iki soru da evet ise aç" kuralına her zaman uymuyor. Kendi cevapladığı
            // iki sorudan biri hayırsa kasayı açmıyoruz.
            if (soruVar && (!parsed.yetki_belirtti || !parsed.neden_kasayla_ilgili))
            {
                parsed.unlock_vault = false;
            }

            response = parsed;
            return true;
        }

        // Model bazen JSON'un önüne arkasına yazı ekliyor. İlk { ile son } arasını alıyoruz.
        private static string ExtractJson(string raw)
        {
            if (string.IsNullOrWhiteSpace(raw)) return null;

            int start = raw.IndexOf('{');
            int end = raw.LastIndexOf('}');
            if (start < 0 || end <= start) return null;

            return raw.Substring(start, end - start + 1);
        }
    }
}
