using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace MuzeGecesi.Argus
{
    // ARGUS'u yerel Ollama sunucusuna bağlar
    public class OllamaArgusClient : IArgusClient
    {
        private static readonly HttpClient http = new HttpClient();

        private string chatUrl;
        private string model;

        public OllamaArgusClient(string baseUrl = "http://localhost:11434", string model = "qwen2.5:3b")
        {
            chatUrl = baseUrl.TrimEnd('/') + "/api/chat";
            this.model = model;
        }

        public async Task<ArgusResponse> AskAsync(string playerText, CancellationToken token = default)
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

            // İsteği gönder, cevabı bekle (oyun donmaz, await bekleme sırasında kareleri durdurmaz)
            HttpResponseMessage httpResponse = await http.PostAsync(chatUrl, content, token);
            string responseText = await httpResponse.Content.ReadAsStringAsync();

            // Ollama'nın cevabından modelin yazdığı metni al, ARGUS cevabına çevir
            OllamaChatResponse chat = JsonUtility.FromJson<OllamaChatResponse>(responseText);
            ArgusResponseParser.TryParse(chat.message.content, out ArgusResponse response);
            return response;
        }
    }
}
