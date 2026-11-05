using UnityEngine;
using UnityEngine.SceneManagement;

// Oyunun giris ekrani: Oyuna Basla, Ayarlar, Kontroller, Cikis.
// Arayuzu kendisi kodla kurar, sahnede sadece kamera, EventSystem ve bu bilesen olur.
public class AnaMenu : MonoBehaviour
{
    [SerializeField] private string oyunSahnesi = "muze";

    private GameObject anaPanel;
    private GameObject ayarPaneli;
    private GameObject kontrolPaneli;

    private void Start()
    {
        Ayarlar.Uygula();
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Canvas canvas = ArayuzYardimci.CanvasOlustur("MenuCanvas", 0);
        ArayuzYardimci.Panel(canvas.transform, "Arkaplan", new Color(0.02f, 0.02f, 0.04f, 1f));

        anaPanel = new GameObject("AnaPanel", typeof(RectTransform));
        anaPanel.transform.SetParent(canvas.transform, false);

        ArayuzYardimci.Yazi(anaPanel.transform, "MÜZE GECESİ", 110f, new Vector2(0f, 300f), new Vector2(1400f, 160f), ArayuzYardimci.Altin);
        ArayuzYardimci.Yazi(anaPanel.transform, "Elektrikler kesik. Robot Tozcu'yla sigortayı aç, güvenlik yapay zekası ARGUS'u ikna et ve kasaya ulaş.",
            30f, new Vector2(0f, 190f), new Vector2(1300f, 100f), new Color(0.8f, 0.85f, 0.95f));

        ArayuzYardimci.Dugme(anaPanel.transform, "Oyuna Başla", new Vector2(0f, 40f), OyunaBasla);
        ArayuzYardimci.Dugme(anaPanel.transform, "Ayarlar", new Vector2(0f, -40f), () => PaneliAc(ayarPaneli));
        ArayuzYardimci.Dugme(anaPanel.transform, "Kontroller", new Vector2(0f, -120f), () => PaneliAc(kontrolPaneli));
        ArayuzYardimci.Dugme(anaPanel.transform, "Çıkış", new Vector2(0f, -200f), Cikis);

        ayarPaneli = ArayuzYardimci.AyarPaneli(canvas.transform, () => PaneliAc(null));
        kontrolPaneli = ArayuzYardimci.KontrolPaneli(canvas.transform, () => PaneliAc(null));
    }

    // null verilirse ana panele doner
    private void PaneliAc(GameObject panel)
    {
        ayarPaneli.SetActive(panel == ayarPaneli);
        kontrolPaneli.SetActive(panel == kontrolPaneli);
        anaPanel.SetActive(panel == null);
    }

    private void OyunaBasla()
    {
        SceneManager.LoadScene(oyunSahnesi);
    }

    public static void Cikis()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
