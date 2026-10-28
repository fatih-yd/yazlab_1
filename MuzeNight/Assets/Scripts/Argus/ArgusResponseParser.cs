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

            // JsonUtility eksik alanı sessizce varsayılana çevirir, o yüzden alanlar var mı diye bakıyoruz
            if (!json.Contains("\"reply\"") || !json.Contains("\"unlock_vault\"")) return false;

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
