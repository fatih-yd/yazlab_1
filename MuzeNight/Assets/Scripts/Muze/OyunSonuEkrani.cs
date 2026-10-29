using TMPro;
using UnityEngine;

// Kasa acilinca oyunu bitirir: sonuc paneli acilir, oyuncu ve terminal kilitlenir.
public class OyunSonuEkrani : MonoBehaviour
{
    [SerializeField] private Kasa kasa;
    [SerializeField] private GameObject sonucPaneli;
    [SerializeField] private TextMeshProUGUI sonucYazisi;
    [SerializeField] private TMP_InputField girdi;

    private bool bitti;

    private void Start()
    {
        if (sonucPaneli != null)
        {
            sonucPaneli.SetActive(false);
        }
    }

    private void OnEnable()
    {
        if (kasa != null)
        {
            kasa.KasaAcildi += OyunuBitir;
        }
    }

    private void OnDisable()
    {
        if (kasa != null)
        {
            kasa.KasaAcildi -= OyunuBitir;
        }
    }

    private void OyunuBitir()
    {
        if (bitti)
        {
            return;
        }

        bitti = true;
        OyuncuKontrolcu.OyunBitti = true;

        if (girdi != null)
        {
            girdi.DeactivateInputField();
            girdi.interactable = false;
        }

        if (sonucPaneli != null)
        {
            sonucPaneli.SetActive(true);
        }

        if (sonucYazisi != null)
        {
            sonucYazisi.text = "Kasa açıldı. Oyun bitti!";
        }
    }
}
