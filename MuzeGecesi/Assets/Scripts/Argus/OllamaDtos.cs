using System;
using System.Collections.Generic;

namespace MuzeGecesi.Argus
{
    // Ollama /api/chat için istek ve cevap şekilleri. Alan adları API ile aynı olmak zorunda.
    [Serializable]
    public class OllamaMessage
    {
        public string role;
        public string content;
    }

    [Serializable]
    public class OllamaOptions
    {
        public float temperature;
    }

    [Serializable]
    public class OllamaChatRequest
    {
        public string model;
        public List<OllamaMessage> messages;
        public bool stream;
        public string format;
        public string keep_alive;
        public OllamaOptions options;
    }

    [Serializable]
    public class OllamaChatResponse
    {
        public OllamaMessage message;
    }
}
