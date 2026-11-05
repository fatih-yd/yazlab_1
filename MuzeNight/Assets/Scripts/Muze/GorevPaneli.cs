using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

// Kucuk gorev: Muzedeki eser plakalarini ve notlari bul (baslangic odasinda 3 plaka, yan salonlarda 2 not). Plakalarda ARGUS'u nasil
// ikna edecegine dair ipuclari var. Zorunlu degil, oyuncu istemezse atlayabilir.
// Yazilar Canvas'in altina oyun basinda kodla olusturulur.
public class GorevPaneli : MonoBehaviour
{
    [SerializeField] private float okumaMesafesi = 2f;
    [SerializeField] private float ipucuSuresi = 10f;

    private Eser[] eserler;
    private Transform oyuncu;
    private TextMeshProUGUI sayacYazisi;
    private TextMeshProUGUI istemYazisi;
    private TextMeshProUGUI ipucuYazisi;
    private int toplanan;
    private float ipucuBitis;

    private void Start()
    {
        eserler = FindObjectsByType<Eser>(FindObjectsSortMode.None);

        GameObject oyuncuNesnesi = GameObject.Find("oyuncu");
        if (oyuncuNesnesi != null)
        {
            oyuncu = oyuncuNesnesi.transform;
        }

        // Oyunun kendi Canvas'i (duraklatma menusunun Canvas'i degil)
        GameObject canvasNesnesi = GameObject.Find("Canvas");
        Canvas canvas = canvasNesnesi != null ? canvasNesnesi.GetComponent<Canvas>() : null;
        if (canvas == null || oyuncu == null || eserler.Length == 0)
        {
            Debug.LogWarning("Gorev paneli kurulamadi (Canvas, oyuncu ya da eser yok).");
            enabled = false;
            return;
        }

        SayiyiYaz();
        sayacYazisi = YaziOlustur(canvas.transform, "GorevSayac", new Vector2(0f, 1f), new Vector2(20f, -20f), 26, TextAlignmentOptions.TopLeft);
        istemYazisi = YaziOlustur(canvas.transform, "GorevIstem", new Vector2(0.5f, 0f), new Vector2(0f, 140f), 28, TextAlignmentOptions.Center);
        ipucuYazisi = YaziOlustur(canvas.transform, "GorevIpucu", new Vector2(0.5f, 1f), new Vector2(0f, -80f), 26, TextAlignmentOptions.Center);

        SayaciGuncelle();
        istemYazisi.text = "";
        ipucuYazisi.text = "";
    }

    private void Update()
    {
        if (sayacYazisi == null)
        {
            return;
        }

        if (ipucuBitis > 0f && Time.time > ipucuBitis)
        {
            ipucuYazisi.text = "";
            ipucuBitis = 0f;
        }

        // Terminale yazarken ya da oyun bittiyse okuma yapilmaz
        if (OyuncuKontrolcu.HareketKilitli)
        {
            istemYazisi.text = "";
            return;
        }

        Eser yakin = EnYakinEser();
        if (yakin == null)
        {
            istemYazisi.text = "";
            return;
        }

        istemYazisi.text = "[E] İncele: " + yakin.EserAdi;

        if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            PlakayiOku(yakin);
        }
    }

    private Eser EnYakinEser()
    {
        Eser enYakin = null;
        float enKisa = okumaMesafesi;

        foreach (Eser eser in eserler)
        {
            if (eser == null || !eser.gameObject.activeInHierarchy)
            {
                continue;
            }

            float mesafe = Vector3.Distance(oyuncu.position, eser.transform.position);
            if (mesafe < enKisa)
            {
                enKisa = mesafe;
                enYakin = eser;
            }
        }

        return enYakin;
    }

    private void PlakayiOku(Eser eser)
    {
        string metin = eser.EserAdi + "\n" + eser.Plaka;
        eser.Topla();
        toplanan++;
        SayaciGuncelle();

        if (toplanan >= eserler.Length)
        {
            metin += "\n\nTüm ipuçlarını buldun. ARGUS'u ikna etmek için gereken bilgiye sahipsin.";
        }

        ipucuYazisi.text = metin;
        ipucuBitis = Time.time + ipucuSuresi;
        Debug.Log("Plaka okundu: " + eser.EserAdi + " (" + toplanan + "/" + eserler.Length + ")");
    }

    private void SayaciGuncelle()
    {
        sayacYazisi.text = "İpuçları: " + toplanan + "/" + eserler.Length;
    }

    // Eser sayisini baslangicta bir kere kontrol edip konsola yazar
    private void SayiyiYaz()
    {
        Debug.Log("Gorev: " + eserler.Length + " ipucu var.");
    }

    private TextMeshProUGUI YaziOlustur(Transform ana, string ad, Vector2 capa, Vector2 konum, float boyut, TextAlignmentOptions hiza)
    {
        GameObject g = new GameObject(ad, typeof(RectTransform));
        g.transform.SetParent(ana, false);

        RectTransform rt = g.GetComponent<RectTransform>();
        rt.anchorMin = capa;
        rt.anchorMax = capa;
        rt.pivot = capa;
        rt.anchoredPosition = konum;
        rt.sizeDelta = new Vector2(900f, 200f);

        TextMeshProUGUI yazi = g.AddComponent<TextMeshProUGUI>();
        yazi.fontSize = boyut;
        yazi.alignment = hiza;
        yazi.color = new Color(0.85f, 0.95f, 1f);
        yazi.raycastTarget = false;
        return yazi;
    }
}
