using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace MuzeGecesi.Argus
{
    // ARGUS'u yerel Ollama sunucusuna bağlar. Hata olursa exception fırlatmaz, Fallback cevabı döner.
    public class OllamaArgusClient : IArgusClient
    {
        // Zaman aşımını kendimiz yönettiğimiz için HttpClient'ınki kapalı
        private static readonly HttpClient http = new HttpClient { Timeout = Timeout.InfiniteTimeSpan };

        private string chatUrl;
        private string model;
        private float timeoutSeconds;

        public OllamaArgusClient(string baseUrl = "http://localhost:11434", string model = "qwen2.5:3b", float timeoutSeconds = 10f)
        {
            chatUrl = baseUrl.TrimEnd('/') + "/api/chat";
            this.model = model;
            this.timeoutSeconds = timeoutSeconds;
        }

        public async Task<ArgusResponse> AskAsync(string playerText, CancellationToken token = default)
        {
            if (string.IsNullOrWhiteSpace(playerText))
                return ArgusResponse.Fallback("Oyuncu bos mesaj gonderdi.");

            try
            {
                string body = BuildBody(playerText);
                string responseText = await Post(body, token);
                return ReadResponse(responseText);
            }
            catch (OperationCanceledException)
            {
                // Zaman aşımı da iptal olarak gelir, hangisi olduğuna token'dan bakıyoruz
                if (token.IsCancellationRequested)
                    return ArgusResponse.Fallback("Istek iptal edildi.");
                return ArgusResponse.Fallback("Zaman asimi: Ollama " + timeoutSeconds + " sn icinde cevap vermedi.");
            }
            catch (HttpRequestException e)
            {
                return ArgusResponse.Fallback("Ollama'ya ulasilamadi: " + e.Message);
            }
            catch (Exception e)
            {
                return ArgusResponse.Fallback("Beklenmeyen hata: " + e.Message);
            }
        }

        private string BuildBody(string playerText)
        {
            OllamaChatRequest request = new OllamaChatRequest();
            request.model = model;
            request.messages = new List<OllamaMessage>();
            request.messages.Add(new OllamaMessage { role = "system", content = ArgusPrompt.SystemPrompt });
            request.messages.Add(new OllamaMessage { role = "user", content = playerText });
            request.stream = false;
            request.format = "json";
            request.options = new OllamaOptions { temperature = 0.2f };
            return JsonUtility.ToJson(request);
        }

        private async Task<string> Post(string body, CancellationToken token)
        {
            using (CancellationTokenSource cts = CancellationTokenSource.CreateLinkedTokenSource(token))
            using (StringContent content = new StringContent(body, Encoding.UTF8, "application/json"))
            {
                cts.CancelAfter(TimeSpan.FromSeconds(timeoutSeconds));

                using (HttpResponseMessage response = await http.PostAsync(chatUrl, content, cts.Token))
                {
                    // Model yüklü değilse Ollama 404 döner
                    if (!response.IsSuccessStatusCode)
                        throw new HttpRequestException("Ollama hata kodu dondu: " + (int)response.StatusCode);

                    return await response.Content.ReadAsStringAsync();
                }
            }
        }

        private ArgusResponse ReadResponse(string responseText)
        {
            OllamaChatResponse chat;
            try
            {
                chat = JsonUtility.FromJson<OllamaChatResponse>(responseText);
            }
            catch (ArgumentException)
            {
                return ArgusResponse.Fallback("Ollama cevabi okunamadi.");
            }

            string modelOutput = chat != null && chat.message != null ? chat.message.content : null;

            ArgusResponse response;
            if (!ArgusResponseParser.TryParse(modelOutput, out response))
                return ArgusResponse.Fallback("Modelin cevabi gecerli JSON degil: " + modelOutput);

            return response;
        }
    }
}
