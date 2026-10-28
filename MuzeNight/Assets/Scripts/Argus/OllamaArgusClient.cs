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

        public OllamaArgusClient(string baseUrl, string model, float timeoutSeconds)
        {
            this.chatUrl = baseUrl + "/api/chat";
            this.model = model;
            this.timeoutSeconds = timeoutSeconds;
        }

        public async Task<ArgusResponse> AskAsync(string playerText)
        {
            if (string.IsNullOrWhiteSpace(playerText))
            {
                return ArgusResponse.Fallback("Boş mesaj gönderildi.");
            }

            try
            {
                // Ollama'ya gidecek isteği hazırla
                OllamaChatRequest request = new OllamaChatRequest();
                request.model = model;
                request.messages = new List<OllamaMessage>();
                request.messages.Add(new OllamaMessage { role = "system", content = ArgusPrompt.SystemPrompt });
                request.messages.Add(new OllamaMessage { role = "user", content = playerText });
                request.stream = false;
                request.format = "json";
                request.options = new OllamaOptions { temperature = 0.2f };

                string body = JsonUtility.ToJson(request);
                StringContent content = new StringContent(body, Encoding.UTF8, "application/json");

                // Süre dolunca istek iptal olur
                CancellationTokenSource timer = new CancellationTokenSource(TimeSpan.FromSeconds(timeoutSeconds));

                // await bekleme sırasında oyunu dondurmaz
                HttpResponseMessage httpResponse = await http.PostAsync(chatUrl, content, timer.Token);

                // Model yüklü değilse Ollama 404 döner
                if (!httpResponse.IsSuccessStatusCode)
                {
                    return ArgusResponse.Fallback("Ollama hata kodu döndü: " + (int)httpResponse.StatusCode);
                }

                string responseText = await httpResponse.Content.ReadAsStringAsync();
                OllamaChatResponse chat = JsonUtility.FromJson<OllamaChatResponse>(responseText);

                ArgusResponse response;
                if (ArgusResponseParser.TryParse(chat.message.content, out response))
                {
                    return response;
                }
                return ArgusResponse.Fallback("Modelin cevabı geçerli JSON değil: " + chat.message.content);
            }
            catch (OperationCanceledException)
            {
                return ArgusResponse.Fallback("Zaman aşımı: Ollama " + timeoutSeconds + " sn içinde cevap vermedi.");
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
