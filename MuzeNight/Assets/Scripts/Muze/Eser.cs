using UnityEngine;

// Kaidenin ustundeki toplanabilir eser. Oyuncu yaklasip E'ye basinca plakasi okunur
// ve eser kaldirilir. Plakadaki ipucu gorev panelinde gosterilir.
public class Eser : MonoBehaviour
{
    [SerializeField] private string eserAdi = "Eser";
    [TextArea]
    [SerializeField] private string plaka = "";

    public string EserAdi
    {
        get { return eserAdi; }
    }

    public string Plaka
    {
        get { return plaka; }
    }

    // Editor script'i ad ve plaka metnini buradan verir
    public void Ayarla(string yeniAd, string yeniPlaka)
    {
        eserAdi = yeniAd;
        plaka = yeniPlaka;
    }

    public void Topla()
    {
        gameObject.SetActive(false);
    }
}
