# Sözleşme

Oyun tarafı (sahne, robot, kasa) ile ARGUS tarafı (Ollama, system prompt, JSON) bu dosyadaki kurallara göre konuşur.

## 1. Oyun → Ollama (istek)

- Adres: `POST http://localhost:11434/api/chat`
- Gövde:

```json
{
  "model": "qwen2.5:7b-instruct",
  "messages": [
    { "role": "system", "content": "<ARGUS system prompt>" },
    { "role": "user", "content": "<oyuncunun yazdığı metin>" }
  ],
  "stream": false,
  "format": "json",
  "options": { "temperature": 0.2 }
}
```

- Zaman aşımı: 10 saniye.
- Model adı önerilen değer. İkimiz de onaylayınca bu dosya güncellenir.

## 2. ARGUS → Oyun (cevap)

Ollama'nın `message.content` alanındaki JSON şu yapıda olmalı:

```json
{ "reply": "string", "unlock_vault": true }
```

- `reply`: NPC'nin söyleyeceği metin. Ekranda gösterilir.
- `unlock_vault`: `true` veya `false`.

## 3. Hata durumu

Şunlardan biri olursa: bağlantı yok, zaman aşımı, JSON bozuk, alan eksik.

- `unlock_vault = false`
- `reply = "Şu an konuşamıyorum."`
- Oyun çökmez, kullanıcıya bu mesaj gösterilir.

## 4. Oyun kuralı

- Kasa yalnızca **hem** `unlock_vault == true` **hem de** elektrik açıksa açılır.
- ARGUS tek başına kasayı açamaz.

## 5. Arayüz (ayrım)

- ARGUS tarafı `IArgusClient` gibi bir arayüz açar.
- Sahne tarafı bu arayüzü kullanır. Gerçek ARGUS yerine sahte bir sınıf da takılabilir.
- Böylece iki taraf birbirini beklemeden geliştirebilir.
