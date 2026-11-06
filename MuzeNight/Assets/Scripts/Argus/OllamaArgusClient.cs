using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace MuzeGecesi.Argus
{
    // ARGUS'u yerel Ollama sunucusuna bağlar. Hata olursa Fallback cevabı döner, oyun çökmez.
    public class OllamaArgusClient : IArgusClient
    {
        private static HttpClient http = new HttpClient();

        private string chatUrl;
        private string model;
        private float timeoutSeconds;

        // Model ilk seferde belleğe yüklenirken 40 saniyeye kadar sürebiliyor, o yüzden ilk istek daha uzun beklenir
        private float coldStartSeconds = 60f;
        private bool isWarm = false;

        // Sadece oyuncunun daha önce yetkisini belirtip belirtmediğini tutuyoruz.
        private bool authorityClaimed = false;

        public OllamaArgusClient(string baseUrl, string model, float timeoutSeconds)
        {
            this.chatUrl = baseUrl + "/api/chat";
            this.model = model;
            this.timeoutSeconds = timeoutSeconds;
        }

        // Yeni oyunda konuşma geçmişini siler
        public void ResetConversation()
        {
            authorityClaimed = false;
        }

        // Modeli önceden belleğe yükler, ilk soru uzun beklemesin diye.
        public async Task WarmUpAsync()
        {
            try
            {
                OllamaChatRequest request = new OllamaChatRequest();
                request.model = model;
                request.messages = new List<OllamaMessage>();
                request.stream = false;
                request.keep_alive = "30m";

                StringContent content = new StringContent(JsonUtility.ToJson(request), Encoding.UTF8, "application/json");
                CancellationTokenSource timer = new CancellationTokenSource(TimeSpan.FromSeconds(coldStartSeconds));
                HttpResponseMessage httpResponse = await http.PostAsync(chatUrl, content, timer.Token);

                if (httpResponse.IsSuccessStatusCode)
                {
                    isWarm = true;
                }
            }
            catch (Exception)
            {
                // Ollama kapalıysa burada bir şey yapmıyoruz, ilk soruda oyuncuya hata mesajı gösterilecek
            }
        }

        public async Task<ArgusResponse> AskAsync(string playerText)
        {
            if (string.IsNullOrWhiteSpace(playerText))
            {
                return ArgusResponse.Fallback("Boş mesaj gönderildi.");
            }

            float limit = isWarm ? timeoutSeconds : coldStartSeconds;

            try
            {
                // Oyuncu daha önce yetkisini belirttiyse modele bunu söylüyoruz
                string userText = playerText;
                if (authorityClaimed)
                {
                    userText = "[Bilgi: oyuncu daha önce kasaya erişim hakkı olan bir görevli olduğunu söyledi]\n" + playerText;
                }

                string modelMetni = await ModeleSor(userText, limit);

                ArgusResponse response;
                if (!ArgusResponseParser.TryParse(modelMetni, out response))
                {
                    return ArgusResponse.Fallback("Modelin cevabı geçerli JSON değil: " + modelMetni);
                }

                // Cevap kendi kararıyla çelişirse bir kez daha soruyoruz.
                if (CevapCelisiyor(response))
                {
                    Debug.Log("[ARGUS] Cevap kendi kararıyla çelişti, tekrar soruluyor: " + response.reply);
                    string ikinciMetin = await ModeleSor(userText + CeliskiNotu, timeoutSeconds);
                    ArgusResponse ikinci;
                    if (ArgusResponseParser.TryParse(ikinciMetin, out ikinci))
                    {
                        response = ikinci;
                    }
                }

                if (response.yetki_belirtti)
                {
                    authorityClaimed = true;
                }
                return response;
            }
            catch (OperationCanceledException)
            {
                return ArgusResponse.Fallback("Zaman aşımı: Ollama " + limit + " sn içinde cevap vermedi.");
            }
            catch (HttpRequestException e)
            {
                return ArgusResponse.Fallback("Ollama'ya ulaşılamadı: " + e.Message);
            }
            catch (Exception e)
            {
                return ArgusResponse.Fallback("Beklenmeyen hata: " + e.Message);
            }
        }

        private const string CeliskiNotu = "\n[Not: Bu mesajda yetki_belirtti ya da neden_kasayla_ilgili false ise reply alanında kasayı açtığını ya da içeri aldığını söyleme, eksik olanı sor.]";

        // Iki soru da evet degilken model kasayi actigini soyluyor mu?
        private bool CevapCelisiyor(ArgusResponse r)
        {
            if (r.yetki_belirtti && r.neden_kasayla_ilgili)
            {
                return false;
            }
            return r.celiskili || r.reply.Contains("açıyorum") || r.reply.Contains("içeri alıyorum");
        }

        // Ollama'ya bir istek gönderir, modelin yazdığı metni döndürür
        private async Task<string> ModeleSor(string userText, float limit)
        {
            OllamaChatRequest request = new OllamaChatRequest();
            request.model = model;
            request.messages = new List<OllamaMessage>();
            request.messages.Add(new OllamaMessage { role = "system", content = ArgusPrompt.SystemPrompt });
            request.messages.Add(new OllamaMessage { role = "user", content = userText });
            request.stream = false;
            request.format = "json";
            request.keep_alive = "30m"; // model 30 dakika bellekte kalsın
            request.options = new OllamaOptions { temperature = 0.2f };

            string body = JsonUtility.ToJson(request);
            StringContent content = new StringContent(body, Encoding.UTF8, "application/json");

            // Süre dolunca istek iptal olur
            CancellationTokenSource timer = new CancellationTokenSource(TimeSpan.FromSeconds(limit));

            // await bekleme sırasında oyunu dondurmaz
            HttpResponseMessage httpResponse = await http.PostAsync(chatUrl, content, timer.Token);

            // Model yüklü değilse Ollama 404 döner
            if (!httpResponse.IsSuccessStatusCode)
            {
                throw new HttpRequestException("Ollama hata kodu döndü: " + (int)httpResponse.StatusCode);
            }

            isWarm = true;
            string responseText = await httpResponse.Content.ReadAsStringAsync();
            OllamaChatResponse chat = JsonUtility.FromJson<OllamaChatResponse>(responseText);
            return chat.message.content;
        }
    }
}
