using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

// Oyuncunun ARGUS ile yazistigi terminal.
// Enter'a basinca mesaji gonderir, cevabi gosterir ve kasa kararini uygular.
public class TerminalKontrolcu : MonoBehaviour
{
    [SerializeField] private TMP_InputField girdi;
    [SerializeField] private TextMeshProUGUI cikti;
    [SerializeField] private Kasa kasa;

    private ICevapKaynagi kaynak;
    private bool mesgulMu;

    private void Awake()
    {
        kaynak = GetComponent<ICevapKaynagi>();
        girdi.onSubmit.AddListener(EnterBasildi);
    }

    private void Update()
    {
        // Yazi yazilirken oyuncu hareket etmesin
        OyuncuKontrolcu.HareketKilitli = girdi.isFocused;

        // Escape ile yazi kutusundan cik, boylece G tusu Tozcu komutu olarak calisir
        if (girdi.isFocused && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            girdi.DeactivateInputField();
        }
    }

    // Enter'a basildiginda TMP bu metodu cagirir
    private void EnterBasildi(string _)
    {
        if (mesgulMu)
        {
            return;
        }

        MesajiGonder();
    }

    private async void MesajiGonder()
    {
        string metin = girdi.text.Trim();
        if (string.IsNullOrEmpty(metin))
        {
            return;
        }

        mesgulMu = true;
        Debug.Log("Oyuncu: " + metin);
        girdi.text = "";
        girdi.ActivateInputField();
        cikti.text = "ARGUS düşünüyor...";

        ArgusCevap cevap;
        try
        {
            cevap = await kaynak.CevapAlAsync(metin);
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

        cikti.text = "ARGUS: " + cevap.reply;
        Debug.Log("ARGUS: " + cevap.reply + " | kasa izni: " + cevap.unlock_vault);

        if (kasa != null)
        {
            kasa.ArgusKarariniUygula(cevap.unlock_vault);
        }

        mesgulMu = false;
    }
}
