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

    [SerializeField] private float etkilesimMesafesi = 2.6f;

    private bool bitti;
    private bool kasaAcik;
    private Transform oyuncu;
    private TextMeshProUGUI istemYazisi;

    private void Start()
    {
        if (sonucPaneli != null)
        {
            sonucPaneli.SetActive(false);
        }

        GameObject oyuncuNesnesi = GameObject.Find("oyuncu");
        if (oyuncuNesnesi != null)
        {
            oyuncu = oyuncuNesnesi.transform;
        }

        Canvas canvas = ArayuzYardimci.CanvasOlustur("KasaCanvas", 20);
        istemYazisi = ArayuzYardimci.Yazi(canvas.transform, "", 30f, new Vector2(0f, -120f), new Vector2(900f, 60f), ArayuzYardimci.Altin);
    }

    private void OnEnable()
    {
        if (kasa != null)
        {
            kasa.KasaAcildi += KasaAcildiginda;
        }
    }

    private void OnDisable()
    {
        if (kasa != null)
        {
            kasa.KasaAcildi -= KasaAcildiginda;
        }
    }

    // Kasa acilinca oyun hemen bitmez: kapi acilir, iceriden altin bir isik yayilir,
    // oyuncu kasanin yanina gidip E'ye basinca oyun biter.
    private void KasaAcildiginda()
    {
        kasaAcik = true;

        // Terminalden cik ki oyuncu yuruyebilsin
        if (girdi != null)
        {
            girdi.DeactivateInputField();
        }

        StartCoroutine(AltinIsik());
    }

    private IEnumerator AltinIsik()
    {
        GameObject g = new GameObject("KasaIsigi");
        g.transform.position = kasa.transform.position + new Vector3(-1f, 0.3f, 0f);
        Light isik = g.AddComponent<Light>();
        isik.type = LightType.Point;
        isik.color = new Color(1f, 0.75f, 0.3f);
        isik.range = 4f;

        float gecen = 0f;
        while (gecen < 1.5f)
        {
            gecen += Time.deltaTime;
            isik.intensity = Mathf.Lerp(0f, 2.5f, gecen / 1.5f);
            yield return null;
        }
    }

    private void Update()
    {
        if (!kasaAcik || bitti || oyuncu == null || istemYazisi == null)
        {
            return;
        }

        bool yakin = Vector3.Distance(oyuncu.position, kasa.transform.position) < etkilesimMesafesi;
        istemYazisi.text = yakin ? "[E] Kasadaki eserleri al" : "";

        if (yakin && !OyuncuKontrolcu.HareketKilitli && UnityEngine.InputSystem.Keyboard.current != null
            && UnityEngine.InputSystem.Keyboard.current.eKey.wasPressedThisFrame)
        {
            istemYazisi.text = "";
            gosterimGecikmesi = 0.3f;
            OyunuBitir();
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
            sonucYazisi.text = "Firavun maskesini ve kasadaki eserleri kurtardın.\nOyun bitti! (Ana menü için Esc)";
        }
    }
}
