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
    private GuvenGostergesi gosterge;

    // Guven puani icin: oyuncu bir kez yetkisini belirttiyse hatirlanir
    private bool yetkiVar;
    private int guven;

    private void Awake()
    {
        istemci = new OllamaArgusClient(ollamaUrl, modelAdi, zamanAsimiSaniye);
        gosterge = gameObject.AddComponent<GuvenGostergesi>();
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
        else
        {
            // Guven puani modelin cevapladigi iki sorudan hesaplanir:
            // yetki 50, yetkiyle birlikte acil neden +40 (yetkisiz neden 25), izin verildiyse 100
            if (cevap.yetki_belirtti)
            {
                yetkiVar = true;
            }

            guven = 0;
            if (yetkiVar)
            {
                guven += 50;
            }
            if (cevap.neden_kasayla_ilgili)
            {
                guven += yetkiVar ? 40 : 25;
            }
            if (cevap.unlock_vault)
            {
                guven = 100;
            }
        }

        if (gosterge != null)
        {
            gosterge.Guncelle(guven, yetkiVar, !cevap.isError && cevap.neden_kasayla_ilgili, cevap.isError);
        }

        // Terminalin kullandığı sınıfa çevir
        ArgusCevap sonuc = new ArgusCevap();
        sonuc.reply = cevap.reply;
        sonuc.unlock_vault = cevap.unlock_vault;
        return sonuc;
    }
}
