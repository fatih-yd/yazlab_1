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

        // Oyuncunun önceki mesajlarını tutuyoruz (son 4). ARGUS'un eski cevaplarını tutmuyoruz,
        // çünkü model eski retleri tekrar edip yeni mesajı da reddediyordu.
        private List<string> history = new List<string>();

        public OllamaArgusClient(string baseUrl, string model, float timeoutSeconds)
        {
            this.chatUrl = baseUrl + "/api/chat";
            this.model = model;
            this.timeoutSeconds = timeoutSeconds;
        }

        // Yeni oyunda konuşma geçmişini siler
        public void ResetConversation()
        {
            history.Clear();
        }

        // Modeli önceden belleğe yükler, ilk soru uzun beklemesin diye.
        // Mesaj listesi boş gönderilirse Ollama cevap üretmez, sadece modeli yükler.
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
                // Ollama'ya gidecek isteği hazırla
                OllamaChatRequest request = new OllamaChatRequest();
                request.model = model;
                request.messages = new List<OllamaMessage>();
                request.messages.Add(new OllamaMessage { role = "system", content = ArgusPrompt.SystemPrompt });

                // Önceki mesajlar varsa onları da aynı mesajın içine yazıyoruz
                string userText = playerText;
                if (history.Count > 0)
                {
                    userText = "Oyuncunun önceki mesajları: " + string.Join(" | ", history) + "\nOyuncunun şimdiki mesajı: " + playerText;
                }
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
                    return ArgusResponse.Fallback("Ollama hata kodu döndü: " + (int)httpResponse.StatusCode);
                }

                isWarm = true;
                string responseText = await httpResponse.Content.ReadAsStringAsync();
                OllamaChatResponse chat = JsonUtility.FromJson<OllamaChatResponse>(responseText);

                ArgusResponse response;
                if (ArgusResponseParser.TryParse(chat.message.content, out response))
                {
                    history.Add(playerText);
                    if (history.Count > 4)
                    {
                        history.RemoveAt(0);
                    }
                    return response;
                }
                return ArgusResponse.Fallback("Modelin cevabı geçerli JSON değil: " + chat.message.content);
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
    }
}
