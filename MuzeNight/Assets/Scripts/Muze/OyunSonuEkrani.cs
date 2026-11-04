using System.Collections;
using TMPro;
using UnityEngine;

// Kasa acilinca oyunu bitirir: oyuncu ve terminal hemen kilitlenir,
// sonuc paneli kapi animasyonu izlensin diye kisa bir gecikmeyle acilir.
public class OyunSonuEkrani : MonoBehaviour
{
    [SerializeField] private Kasa kasa;
    [SerializeField] private GameObject sonucPaneli;
    [SerializeField] private TextMeshProUGUI sonucYazisi;
    [SerializeField] private TMP_InputField girdi;
    [SerializeField] private float gosterimGecikmesi = 2.5f;

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

        StartCoroutine(SonucuGoster());
    }

    // Kapi acilirken ekran kapanmasin, animasyon bitince sonuc gosterilsin
    private IEnumerator SonucuGoster()
    {
        yield return new WaitForSeconds(gosterimGecikmesi);

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
