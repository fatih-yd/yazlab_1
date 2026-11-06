using System;
using System.Collections;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

// Oyuncunun ARGUS ile yazistigi terminal.
public class TerminalKontrolcu : MonoBehaviour
{
    // Karsilama cumlesi sabit degil: modele durum anlatilir, selami ve soruyu model kendisi uretir
    private const string KarsilamaIstegi =
        "[Oyun bilgisi: Gece yarisi, muze kapali. Biri az once kasanin onundeki terminale yaklasti. " +
        "Onu kisaca karsila; kim oldugunu ve muzenin bu saatinde burada ne aradigini sor.]";

    [SerializeField] private TMP_InputField girdi;
    [SerializeField] private TextMeshProUGUI cikti;
    [SerializeField] private Kasa kasa;
    [SerializeField] private float yazmaHizi = 45f;

    private ICevapKaynagi kaynak;
    private ArgusGorunurluk gorunurluk;
    private Coroutine daktilo;
    private bool mesgulMu;

    private void Awake()
    {
        kaynak = GetComponent<ICevapKaynagi>();
        girdi.onSubmit.AddListener(EnterBasildi);

        // Uzun cevaplar panelden tasmasin: satir kaydir, sigmazsa yaziyi kucult
        cikti.textWrappingMode = TextWrappingModes.Normal;
        cikti.enableAutoSizing = true;
        cikti.fontSizeMin = 16f;
        cikti.fontSizeMax = 26f;

        gorunurluk = GetComponent<ArgusGorunurluk>();
        if (gorunurluk != null)
        {
            gorunurluk.Belirdi += KarsilamaYap;
        }
    }

    private void OnDestroy()
    {
        if (gorunurluk != null)
        {
            gorunurluk.Belirdi -= KarsilamaYap;
        }
    }

    private void Update()
    {
        // Yazi yazilirken oyuncu hareket etmesin
        OyuncuKontrolcu.HareketKilitli = girdi.isFocused;

        if (girdi.isFocused)
        {
            // Escape ile terminalden cik, oyuna (fareyle bakisa) don
            if (Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                girdi.DeactivateInputField();
            }
        }
        else if (Keyboard.current.enterKey.wasPressedThisFrame
                 && girdi.gameObject.activeInHierarchy
                 && girdi.interactable)
        {
            // Imlec kilitliyken kutuya tiklanamaz, terminal Enter ile acilir
            girdi.ActivateInputField();
        }
    }

    // ARGUS belirince ilk sozu o soyler; bu konusma kasa kararini etkilemez
    private async void KarsilamaYap()
    {
        if (mesgulMu)
        {
            return;
        }

        await Konus(KarsilamaIstegi, null, false);
    }

    // Enter'a basildiginda TMP bu metodu cagirir
    private async void EnterBasildi(string _)
    {
        if (mesgulMu)
        {
            return;
        }

        string metin = girdi.text.Trim();
        if (string.IsNullOrEmpty(metin))
        {
            return;
        }

        girdi.text = "";
        girdi.ActivateInputField();
        Debug.Log("Oyuncu: " + metin);

        await Konus(metin, metin, true);
    }

    // Kaynaga sorar, cevabi yazar; kararUygula true ise kasa kararini da uygular
    private async Task Konus(string istek, string oyuncuMetni, bool kararUygula)
    {
        mesgulMu = true;

        string onEk = OyuncuSatiri(oyuncuMetni);
        DaktiloyuDurdur();
        cikti.text = onEk + "<i><color=#8C8C8C>ARGUS düşünüyor...</color></i>";

        ArgusCevap cevap;
        try
        {
            cevap = await kaynak.CevapAlAsync(istek);
        }
        catch (Exception hata)
        {
            // Ollama kapali, zaman asimi ya da baska bir hata: oyun cokmez, kasa kapali kalir
            Debug.LogWarning("ARGUS hatasi: " + hata.Message);
            cevap = ArgusCevap.Yedek();
        }

        if (cevap == null)
        {
            cevap = ArgusCevap.Yedek();
        }

        daktilo = StartCoroutine(DaktiloYaz(onEk, cevap.reply ?? ""));
        Debug.Log("ARGUS: " + cevap.reply + " | kasa izni: " + cevap.unlock_vault);

        if (kararUygula && kasa != null)
        {
            kasa.ArgusKarariniUygula(cevap.unlock_vault);
        }

        mesgulMu = false;
    }

    // Oyuncunun son mesaji gri, kucuk bir satir olarak cevabin ustunde durur
    private static string OyuncuSatiri(string oyuncuMetni)
    {
        if (string.IsNullOrEmpty(oyuncuMetni))
        {
            return "";
        }

        return "<size=80%><color=#8C8C8C>Sen: " + Temizle(oyuncuMetni) + "</color></size>\n";
    }

    // Metindeki < > isaretleri bicim etiketi sanilmasin diye benzer isaretlerle degistirilir.
    private static string Temizle(string metin)
    {
        return metin.Replace("<", "‹").Replace(">", "›");
    }

    // Cevap harf harf yazilir
    private IEnumerator DaktiloYaz(string onEk, string metin)
    {
        float bekleme = 1f / Mathf.Max(1f, yazmaHizi);
        metin = Temizle(metin);

        for (int i = 1; i <= metin.Length; i++)
        {
            cikti.text = onEk + metin.Substring(0, i);
            yield return new WaitForSeconds(bekleme);
        }

        daktilo = null;
    }

    private void DaktiloyuDurdur()
    {
        if (daktilo != null)
        {
            StopCoroutine(daktilo);
            daktilo = null;
        }
    }
}
