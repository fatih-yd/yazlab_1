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
- Önceki mesajların hepsi gönderilmez, çünkü model eski mesajlara cevap veriyor ve eski retleri tekrarlıyordu. Sadece oyuncunun daha önce yetkisini belirtip belirtmediği (`yetki_belirtti`) hatırlanır. Belirttiyse şimdiki mesajın başına `[Bilgi: oyuncu daha önce kasaya erişim hakkı olan bir görevli olduğunu söyledi]` satırı eklenir. Böylece oyuncu önce yetkisini, sonra nedenini ayrı mesajlarda söyleyebilir.

## 2. ARGUS → Oyun (cevap)

Ollama'nın `message.content` alanındaki JSON şu yapıda olmalı:

```json
{ "yetki_belirtti": true, "neden_kasayla_ilgili": true, "reply": "string", "unlock_vault": true }
```

- `yetki_belirtti`: Oyuncu kasaya erişim hakkı olan bir görevli olduğunu söyledi mi.
- `neden_kasayla_ilgili`: Oyuncunun nedeni kasadaki eserlerle ilgili acil bir durum mu.
- `reply`: NPC'nin söyleyeceği metin. Ekranda gösterilir.
- `unlock_vault`: `true` veya `false`. Model bunu yalnızca iki soru da `true` ise `true` yapmalı. Model uymazsa oyun tarafı da kontrol eder, iki sorudan biri `false` ise kasa açılmaz.
- Model bazen `unlock_vault` alanını yazmayı unutuyor. İki soru cevaplanmışsa ve `reply` varsa bu hata sayılmaz, eksik `unlock_vault` `false` kabul edilir.
- Oyun yalnızca `reply` ve `unlock_vault` alanlarını kullanır. İlk iki alan modelin daha doğru karar vermesi içindir.

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
