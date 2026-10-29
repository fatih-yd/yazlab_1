using System.Threading.Tasks;
using UnityEngine;

// Gelistirme icin sahte ARGUS. Ollama baglanana kadar terminali test etmeye yarar.
// Son surumde cevaplar Ollama'dan gelmeli, bu sinif sadece test icindir.
public class SahteArgusKaynagi : MonoBehaviour, ICevapKaynagi
{
    // Test icin: isaretliyse sahte ARGUS kasayi acmaya izin verir.
    [SerializeField] private bool testKasayiAcsin;

    public async Task<ArgusCevap> CevapAlAsync(string oyuncuMetni)
    {
        // Gercek bir modelin dusunme suresini taklit eder
        await Task.Delay(800);

        return new ArgusCevap
        {
            reply = "Bu talebi değerlendiriyorum. [sahte cevap]",
            unlock_vault = testKasayiAcsin
        };
    }
}
