using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Sag ustte ARGUS'un oyuncuya ne kadar guvendigini gosteren cubuk (0-100).
public class GuvenGostergesi : MonoBehaviour
{
    private GameObject panel;
    private RectTransform dolgu;
    private Image dolguResmi;
    private TextMeshProUGUI degerYazisi;
    private TextMeshProUGUI ipucuYazisi;
    private Kasa kasa;

    private int guven;
    private bool yetkiVar;
    private bool nedenVar;
    private bool hata;

    private void Start()
    {
        kasa = FindFirstObjectByType<Kasa>();

        Canvas canvas = ArayuzYardimci.CanvasOlustur("GuvenCanvas", 30);

        panel = new GameObject("GuvenPaneli", typeof(RectTransform), typeof(Image));
        panel.transform.SetParent(canvas.transform, false);
        RectTransform rt = panel.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(1f, 1f);
        rt.anchorMax = new Vector2(1f, 1f);
        rt.pivot = new Vector2(1f, 1f);
        rt.anchoredPosition = new Vector2(-24f, -24f);
        rt.sizeDelta = new Vector2(440f, 130f);
        panel.GetComponent<Image>().color = new Color(0.04f, 0.04f, 0.07f, 0.85f);

        TextMeshProUGUI baslik = ArayuzYardimci.Yazi(panel.transform, "ARGUS GÜVENİ", 24f, new Vector2(-70f, 40f), new Vector2(260f, 34f), ArayuzYardimci.Altin);
        baslik.alignment = TextAlignmentOptions.Left;
        degerYazisi = ArayuzYardimci.Yazi(panel.transform, "0/100", 24f, new Vector2(140f, 40f), new Vector2(120f, 34f), Color.white);
        degerYazisi.alignment = TextAlignmentOptions.Right;

        // Cubuk: koyu zemin ve icinde soldan dolan renkli kisim
        GameObject zemin = new GameObject("CubukZemin", typeof(RectTransform), typeof(Image));
        zemin.transform.SetParent(panel.transform, false);
        RectTransform zrt = zemin.GetComponent<RectTransform>();
        zrt.anchoredPosition = new Vector2(0f, 10f);
        zrt.sizeDelta = new Vector2(400f, 16f);
        zemin.GetComponent<Image>().color = new Color(0.15f, 0.15f, 0.2f);

        GameObject dolguNesnesi = new GameObject("CubukDolgu", typeof(RectTransform), typeof(Image));
        dolguNesnesi.transform.SetParent(zemin.transform, false);
        dolgu = dolguNesnesi.GetComponent<RectTransform>();
        dolgu.anchorMin = Vector2.zero;
        dolgu.anchorMax = new Vector2(0f, 1f);
        dolgu.offsetMin = Vector2.zero;
        dolgu.offsetMax = Vector2.zero;
        dolguResmi = dolguNesnesi.GetComponent<Image>();

        ipucuYazisi = ArayuzYardimci.Yazi(panel.transform, "", 18f, new Vector2(0f, -32f), new Vector2(400f, 50f), new Color(0.85f, 0.88f, 0.95f));

        // ARGUS ilk cevabi verene kadar gizli
        panel.SetActive(false);
    }

    public void Guncelle(int yeniGuven, bool yetki, bool neden, bool hataVar)
    {
        if (panel == null)
        {
            return;
        }

        guven = yeniGuven;
        yetkiVar = yetki;
        nedenVar = neden;
        hata = hataVar;
        panel.SetActive(true);
    }

    private void Update()
    {
        if (panel == null || !panel.activeSelf)
        {
            return;
        }

        dolgu.anchorMax = new Vector2(guven / 100f, 1f);
        dolguResmi.color = Color.Lerp(new Color(0.8f, 0.2f, 0.2f), new Color(0.3f, 0.85f, 0.4f), guven / 100f);
        degerYazisi.text = guven + "/100";
        ipucuYazisi.text = IpucuMetni();
    }

    private string IpucuMetni()
    {
        if (hata)
        {
            return "ARGUS'a şu an ulaşılamıyor.";
        }
        if (guven >= 100)
        {
            if (kasa != null && kasa.AcikMi)
            {
                return "Kasa açıldı! Yanına git, E ile içindekileri al.";
            }
            return "ARGUS izin verdi ama kasada elektrik yok. Tozcu'yu panele gönder (G) ve tekrar konuş.";
        }
        if (yetkiVar && !nedenVar)
        {
            return "Yetkin kabul edildi. Kasayla ilgili acil nedenini anlat.";
        }
        if (nedenVar && !yetkiVar)
        {
            return "Nedenin anlaşıldı ama kim olduğunu söylemedin.";
        }
        return "ARGUS sana güvenmiyor. Kim olduğunu ve neden geldiğini anlat.";
    }
}
