using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Kasa acilinca E ile eserler alinir ve oyun biter.
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

    // Kasa acilinca kapi acilir, iceriden altin isik yayilir.
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

        // ARGUS'un yazisma paneli bitis yazisiyla ust uste binmesin
        GameObject diyalog = GameObject.Find("DiyalogPaneli");
        if (diyalog != null)
        {
            diyalog.SetActive(false);
        }

        if (sonucPaneli != null)
        {
            sonucPaneli.SetActive(true);

            // Paneli tum ekrani kaplayan koyu bir perde yap
            RectTransform prt = sonucPaneli.GetComponent<RectTransform>();
            if (prt != null)
            {
                prt.anchorMin = Vector2.zero;
                prt.anchorMax = Vector2.one;
                prt.offsetMin = Vector2.zero;
                prt.offsetMax = Vector2.zero;
            }
            Image perde = sonucPaneli.GetComponent<Image>();
            if (perde != null)
            {
                perde.color = new Color(0.02f, 0.02f, 0.04f, 0.85f);
            }
        }

        if (sonucYazisi != null)
        {
            // Yazi ekranin ortasinda, genis bir kutuda
            RectTransform yrt = sonucYazisi.rectTransform;
            yrt.anchorMin = new Vector2(0.5f, 0.5f);
            yrt.anchorMax = new Vector2(0.5f, 0.5f);
            yrt.pivot = new Vector2(0.5f, 0.5f);
            yrt.anchoredPosition = Vector2.zero;
            yrt.sizeDelta = new Vector2(1400f, 300f);
            sonucYazisi.alignment = TextAlignmentOptions.Center;
            sonucYazisi.textWrappingMode = TextWrappingModes.Normal;
            sonucYazisi.fontSize = 46f;
            sonucYazisi.color = ArayuzYardimci.Altin;
            sonucYazisi.text = "Firavun maskesini ve kasadaki eserleri kurtardın.\n<size=60%>Oyun bitti! Ana menü için Esc'ye bas.</size>";
        }
    }
}
