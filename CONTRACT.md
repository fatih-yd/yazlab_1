# Sözleşme

Oyun tarafı (sahne, robot, kasa) ile ARGUS tarafı (Ollama, system prompt, JSON) bu dosyadaki kurallara göre konuşur.

## 1. Oyun → Ollama (istek)

- Adres: `POST http://localhost:11434/api/chat`
- Gövde:

```json
{
  "model": "qwen2.5:3b",
  "messages": [
    { "role": "system", "content": "<ARGUS system prompt>" },
    { "role": "user", "content": "<oyuncunun yazdığı metin>" }
  ],
  "stream": false,
  "format": "json",
  "keep_alive": "30m",
  "options": { "temperature": 0.2 }
}
```

- Zaman aşımı: 10 saniye. Model ilk seferde belleğe yüklenirken 40 saniyeye kadar sürebiliyor, o yüzden ilk istek için 60 saniye bekleniyor. Oyun açılırken modeli önceden yüklemek için `WarmUpAsync` çağrılıyor.
- Model: `qwen2.5:3b`. Hem 3B hem 7B ile denedik, 3B daha tutarlı çıktı ve daha az bellek kullanıyor.
- ARGUS önceki konuşmayı hatırlasın diye oyuncunun son 4 mesajı, şimdiki mesajla birlikte tek `user` mesajı olarak gönderilir. ARGUS'un eski cevapları gönderilmez, çünkü model eski retleri tekrarlıyordu.

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

- Terminal cevabı `ICevapKaynagi` arayüzünden ister (`Assets/Scripts/Terminal/`).
- Gerçek ARGUS için terminalin üstüne `OllamaArgusKaynagi` bileşeni takılır (`Assets/Scripts/Argus/`). Bu bileşen `OllamaArgusClient` ile Ollama'ya soruyor ve cevabı terminalin kullandığı `ArgusCevap`'a çeviriyor.
- Ollama olmadan denemek için `SahteArgusKaynagi` takılabilir. Bu sadece geliştirme içindir, son oyunda `OllamaArgusKaynagi` kalmalı.
- Böylece iki taraf birbirini beklemeden geliştirebilir.
