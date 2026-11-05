using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Menulerdeki dugme, yazi, kaydirici gibi parcalari kodla olusturur.
// Boylece menu sahnesini elle kurmaya gerek kalmaz.
public static class ArayuzYardimci
{
    public static readonly Color Altin = new Color(0.95f, 0.8f, 0.45f);
    private static readonly Color DugmeRengi = new Color(0.1f, 0.12f, 0.18f, 0.95f);

    public static Canvas CanvasOlustur(string ad, int sira)
    {
        GameObject g = new GameObject(ad, typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        Canvas canvas = g.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = sira;

        CanvasScaler olcekleyici = g.GetComponent<CanvasScaler>();
        olcekleyici.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        olcekleyici.referenceResolution = new Vector2(1920f, 1080f);
        olcekleyici.matchWidthOrHeight = 0.5f;
        return canvas;
    }

    // Ekrani kaplayan renkli panel
    public static GameObject Panel(Transform ana, string ad, Color renk)
    {
        GameObject g = new GameObject(ad, typeof(RectTransform), typeof(Image));
        g.transform.SetParent(ana, false);
        Germe(g.GetComponent<RectTransform>(), Vector2.zero);
        g.GetComponent<Image>().color = renk;
        return g;
    }

    // Ekranin ortasina gore konumlanan yazi
    public static TextMeshProUGUI Yazi(Transform ana, string metin, float boyut, Vector2 konum, Vector2 kutu, Color renk)
    {
        GameObject g = new GameObject("Yazi", typeof(RectTransform));
        g.transform.SetParent(ana, false);
        RectTransform rt = g.GetComponent<RectTransform>();
        rt.anchoredPosition = konum;
        rt.sizeDelta = kutu;

        TextMeshProUGUI yazi = g.AddComponent<TextMeshProUGUI>();
        yazi.text = metin;
        yazi.fontSize = boyut;
        yazi.color = renk;
        yazi.alignment = TextAlignmentOptions.Center;
        yazi.raycastTarget = false;
        return yazi;
    }

    public static Button Dugme(Transform ana, string metin, Vector2 konum, UnityAction tiklaninca)
    {
        GameObject g = new GameObject("Dugme_" + metin, typeof(RectTransform), typeof(Image), typeof(Button));
        g.transform.SetParent(ana, false);
        RectTransform rt = g.GetComponent<RectTransform>();
        rt.anchoredPosition = konum;
        rt.sizeDelta = new Vector2(420f, 64f);

        Image arka = g.GetComponent<Image>();
        arka.color = Color.white;

        Button dugme = g.GetComponent<Button>();
        dugme.targetGraphic = arka;
        ColorBlock renkler = dugme.colors;
        renkler.normalColor = DugmeRengi;
        renkler.highlightedColor = new Color(0.25f, 0.33f, 0.5f, 1f);
        renkler.pressedColor = new Color(0.4f, 0.32f, 0.15f, 1f);
        renkler.selectedColor = DugmeRengi;
        dugme.colors = renkler;
        dugme.onClick.AddListener(tiklaninca);

        TextMeshProUGUI yazi = Yazi(g.transform, metin, 32f, Vector2.zero, Vector2.zero, Altin);
        Germe(yazi.rectTransform, Vector2.zero);
        return dugme;
    }

    // Etiketli kaydirici (orn. fare hassasiyeti, ses)
    public static Slider Kaydirici(Transform ana, string etiket, Vector2 konum, float min, float max, float deger, UnityAction<float> degisince)
    {
        Yazi(ana, etiket, 28f, konum + new Vector2(-330f, 0f), new Vector2(300f, 50f), Color.white);

        GameObject kok = new GameObject("Kaydirici_" + etiket, typeof(RectTransform), typeof(Slider));
        kok.transform.SetParent(ana, false);
        RectTransform kokRt = kok.GetComponent<RectTransform>();
        kokRt.anchoredPosition = konum + new Vector2(120f, 0f);
        kokRt.sizeDelta = new Vector2(460f, 24f);

        Image arka = Cocuk(kok.transform, "Arka", new Color(0.15f, 0.15f, 0.2f));
        Germe(arka.rectTransform, Vector2.zero);

        GameObject dolguAlani = new GameObject("DolguAlani", typeof(RectTransform));
        dolguAlani.transform.SetParent(kok.transform, false);
        Germe(dolguAlani.GetComponent<RectTransform>(), Vector2.zero);
        Image dolgu = Cocuk(dolguAlani.transform, "Dolgu", new Color(0.3f, 0.45f, 0.7f));
        dolgu.rectTransform.sizeDelta = Vector2.zero;

        GameObject tutamacAlani = new GameObject("TutamacAlani", typeof(RectTransform));
        tutamacAlani.transform.SetParent(kok.transform, false);
        Germe(tutamacAlani.GetComponent<RectTransform>(), new Vector2(-20f, 0f));
        Image tutamac = Cocuk(tutamacAlani.transform, "Tutamac", Altin);
        tutamac.rectTransform.sizeDelta = new Vector2(20f, 16f);

        Slider kaydirici = kok.GetComponent<Slider>();
        kaydirici.fillRect = dolgu.rectTransform;
        kaydirici.handleRect = tutamac.rectTransform;
        kaydirici.targetGraphic = tutamac;
        kaydirici.minValue = min;
        kaydirici.maxValue = max;
        kaydirici.value = deger;
        kaydirici.onValueChanged.AddListener(degisince);
        return kaydirici;
    }

    // Etiketli acik/kapali kutusu (orn. tam ekran)
    public static Toggle Secim(Transform ana, string etiket, Vector2 konum, bool deger, UnityAction<bool> degisince)
    {
        Yazi(ana, etiket, 28f, konum + new Vector2(-330f, 0f), new Vector2(300f, 50f), Color.white);

        GameObject kok = new GameObject("Secim_" + etiket, typeof(RectTransform), typeof(Toggle));
        kok.transform.SetParent(ana, false);
        RectTransform kokRt = kok.GetComponent<RectTransform>();
        kokRt.anchoredPosition = konum + new Vector2(-90f, 0f);
        kokRt.sizeDelta = new Vector2(40f, 40f);

        Image kutu = Cocuk(kok.transform, "Kutu", new Color(0.15f, 0.15f, 0.2f));
        Germe(kutu.rectTransform, Vector2.zero);
        Image isaret = Cocuk(kutu.transform, "Isaret", Altin);
        Germe(isaret.rectTransform, new Vector2(-12f, -12f));

        Toggle secim = kok.GetComponent<Toggle>();
        secim.targetGraphic = kutu;
        secim.graphic = isaret;
        secim.isOn = deger;
        secim.onValueChanged.AddListener(degisince);
        return secim;
    }

    // Ayarlar paneli: hem ana menude hem duraklatma menusunde ayni
    public static GameObject AyarPaneli(Transform ana, UnityAction geri)
    {
        GameObject panel = Panel(ana, "AyarPaneli", new Color(0.02f, 0.02f, 0.04f, 0.97f));
        Yazi(panel.transform, "AYARLAR", 64f, new Vector2(0f, 260f), new Vector2(800f, 100f), Altin);

        Kaydirici(panel.transform, "Fare hassasiyeti", new Vector2(0f, 120f), 0.02f, 0.4f, Ayarlar.FareHassasiyeti, deger => Ayarlar.FareHassasiyeti = deger);
        Kaydirici(panel.transform, "Ses", new Vector2(0f, 40f), 0f, 1f, Ayarlar.Ses, deger => Ayarlar.Ses = deger);
        Secim(panel.transform, "Tam ekran", new Vector2(0f, -40f), Ayarlar.TamEkran, deger => Ayarlar.TamEkran = deger);

        Dugme(panel.transform, "Geri", new Vector2(0f, -200f), geri);
        panel.SetActive(false);
        return panel;
    }

    // Kontroller paneli
    public static GameObject KontrolPaneli(Transform ana, UnityAction geri)
    {
        GameObject panel = Panel(ana, "KontrolPaneli", new Color(0.02f, 0.02f, 0.04f, 0.97f));
        Yazi(panel.transform, "KONTROLLER", 64f, new Vector2(0f, 300f), new Vector2(800f, 100f), Altin);

        string metin =
            "W A S D  -  Yürü\n" +
            "Fare  -  Etrafa bak\n" +
            "Sol Shift  -  Koş\n" +
            "G  -  Tozcu'ya sigorta paneline gitmesini söyle\n" +
            "E  -  Eser plakasını oku\n" +
            "Enter  -  Terminale yaz / mesajı gönder\n" +
            "Esc  -  Terminalden çık / oyunu duraklat";
        TextMeshProUGUI yazi = Yazi(panel.transform, metin, 32f, new Vector2(0f, 40f), new Vector2(1000f, 420f), Color.white);
        yazi.lineSpacing = 20f;

        Dugme(panel.transform, "Geri", new Vector2(0f, -260f), geri);
        panel.SetActive(false);
        return panel;
    }

    private static Image Cocuk(Transform ana, string ad, Color renk)
    {
        GameObject g = new GameObject(ad, typeof(RectTransform), typeof(Image));
        g.transform.SetParent(ana, false);
        Image resim = g.GetComponent<Image>();
        resim.color = renk;
        return resim;
    }

    // Ana nesneyi kaplayacak sekilde ayarlar, bosluk eksi verilirse kenarlardan iceri ceker
    private static void Germe(RectTransform rt, Vector2 bosluk)
    {
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.sizeDelta = bosluk;
        rt.anchoredPosition = Vector2.zero;
    }
}
