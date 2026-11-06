using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

// Oyun sahnesinde Esc ile acilan duraklatma menusu ve ekranin ortasindaki nisangah.
public class DuraklatmaMenusu : MonoBehaviour
{
    private const string OyunSahnesi = "muze";
    private const string MenuSahnesi = "AnaMenu";

    private GameObject panel;
    private GameObject ayarPaneli;
    private GameObject nisangah;
    private bool oncekiKareKilitliydi;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Kaydol()
    {
        SceneManager.sceneLoaded -= SahneYuklendi;
        SceneManager.sceneLoaded += SahneYuklendi;
    }

    private static void SahneYuklendi(Scene sahne, LoadSceneMode mod)
    {
        if (sahne.name == OyunSahnesi)
        {
            new GameObject("DuraklatmaMenusu").AddComponent<DuraklatmaMenusu>();
        }
    }

    private void Start()
    {
        Ayarlar.Uygula();

        Canvas canvas = ArayuzYardimci.CanvasOlustur("DuraklatmaCanvas", 50);

        nisangah = ArayuzYardimci.Yazi(canvas.transform, "+", 36f, Vector2.zero, new Vector2(40f, 40f), new Color(1f, 1f, 1f, 0.7f)).gameObject;

        panel = ArayuzYardimci.Panel(canvas.transform, "DuraklatmaPaneli", new Color(0.02f, 0.02f, 0.04f, 0.9f));
        ArayuzYardimci.Yazi(panel.transform, "DURAKLATILDI", 72f, new Vector2(0f, 240f), new Vector2(900f, 120f), ArayuzYardimci.Altin);
        ArayuzYardimci.Dugme(panel.transform, "Devam Et", new Vector2(0f, 60f), Devam);
        ArayuzYardimci.Dugme(panel.transform, "Ayarlar", new Vector2(0f, -20f), () => ayarPaneli.SetActive(true));
        ArayuzYardimci.Dugme(panel.transform, "Ana Menü", new Vector2(0f, -100f), AnaMenuyeDon);
        ArayuzYardimci.Dugme(panel.transform, "Çıkış", new Vector2(0f, -180f), AnaMenu.Cikis);
        panel.SetActive(false);

        ayarPaneli = ArayuzYardimci.AyarPaneli(canvas.transform, () => ayarPaneli.SetActive(false));
    }

    private void Update()
    {
        // Terminalden Esc ile cikilan karede menu acilmasin
        bool terminalAcik = OyuncuKontrolcu.HareketKilitli;
        if (!terminalAcik && !oncekiKareKilitliydi && Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (OyuncuKontrolcu.Duraklatildi)
            {
                Devam();
            }
            else
            {
                Duraklat();
            }
        }
        oncekiKareKilitliydi = terminalAcik;

        if (nisangah != null)
        {
            nisangah.SetActive(!OyuncuKontrolcu.Duraklatildi && !terminalAcik && !OyuncuKontrolcu.OyunBitti);
        }
    }

    private void Duraklat()
    {
        OyuncuKontrolcu.Duraklatildi = true;
        Time.timeScale = 0f;
        panel.SetActive(true);
    }

    private void Devam()
    {
        OyuncuKontrolcu.Duraklatildi = false;
        Time.timeScale = 1f;
        panel.SetActive(false);
        ayarPaneli.SetActive(false);
    }

    private void AnaMenuyeDon()
    {
        Devam();
        SceneManager.LoadScene(MenuSahnesi);
    }
}
