using System.Threading.Tasks;
using UnityEngine;
using MuzeGecesi.Argus;

// Terminal ile Ollama arasındaki köprü. Terminaldeki SahteArgusKaynagi yerine bu takılır.
// Terminal cevabı ICevapKaynagi'dan ister, biz de OllamaArgusClient'a soruyoruz.
public class OllamaArgusKaynagi : MonoBehaviour, ICevapKaynagi
{
    [SerializeField] private string ollamaUrl = "http://localhost:11434";
    [SerializeField] private string modelAdi = "qwen2.5:3b";
    [SerializeField] private float zamanAsimiSaniye = 10f;

    private IArgusClient istemci;

    private void Awake()
    {
        istemci = new OllamaArgusClient(ollamaUrl, modelAdi, zamanAsimiSaniye);
    }

    private async void Start()
    {
        // Oyun açılırken modeli belleğe yükle, ilk soru uzun beklemesin
        await istemci.WarmUpAsync();
    }

    public async Task<ArgusCevap> CevapAlAsync(string oyuncuMetni)
    {
        ArgusResponse cevap = await istemci.AskAsync(oyuncuMetni);

        if (cevap.isError)
        {
            Debug.LogWarning("[ARGUS] " + cevap.errorReason);
        }

        // Terminalin kullandığı sınıfa çevir
        ArgusCevap sonuc = new ArgusCevap();
        sonuc.reply = cevap.reply;
        sonuc.unlock_vault = cevap.unlock_vault;
        return sonuc;
    }
}
