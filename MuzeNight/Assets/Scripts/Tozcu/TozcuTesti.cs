using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

// Tekrarlanabilir Tozcu testi. Oyun sahnesinde F9'a basinca baslar.
public class TozcuTesti : MonoBehaviour
{
    private const string OyunSahnesi = "muze";
    private const float ZamanAsimi = 60f;

    // Baslangic noktalari (X, Z). Duvara denk gelirse en yakin yurunebilir noktaya konur.
    private static readonly Vector2[] Baslangiclar =
    {
        new Vector2(-10f, -5f), new Vector2(-10f, 6f), new Vector2(-7f, -9f), new Vector2(-12f, 0f), new Vector2(-3f, -9f),
        new Vector2(-8f, 8f), new Vector2(-14f, -8f), new Vector2(-11f, 3f), new Vector2(-5f, -2f), new Vector2(-9f, -2f)
    };

    // Sahne yeniden yuklenince kaybolmasin diye static
    private static bool calisiyor;
    private static int sira;
    private static List<float> sureler = new List<float>();
    private static List<string> satirlar = new List<string>();

    private TozcuKontrolcu tozcu;
    private TextMeshProUGUI yazi;
    private bool denemeBasladi;
    private float baslangicZamani;

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
            new GameObject("TozcuTesti").AddComponent<TozcuTesti>();
        }
    }

    private void Start()
    {
        tozcu = FindFirstObjectByType<TozcuKontrolcu>();

        Canvas canvas = ArayuzYardimci.CanvasOlustur("TestCanvas", 60);
        yazi = ArayuzYardimci.Yazi(canvas.transform, "", 22f, new Vector2(-560f, 300f), new Vector2(760f, 300f), Color.white);
        yazi.alignment = TextAlignmentOptions.TopLeft;

        if (calisiyor && tozcu != null)
        {
            StartCoroutine(DenemeyiBaslat());
        }
    }

    private IEnumerator DenemeyiBaslat()
    {
        // Diger nesnelerin Start'i bitsin
        yield return null;

        Vector2 xz = Baslangiclar[sira];
        Vector3 hedef = new Vector3(xz.x, 0f, xz.y);
        NavMeshHit nokta;
        if (NavMesh.SamplePosition(hedef, out nokta, 3f, NavMesh.AllAreas))
        {
            tozcu.Agent.Warp(nokta.position);
        }

        baslangicZamani = Time.time;
        tozcu.PaneleGitKomutu();
        denemeBasladi = true;
    }

    private void Update()
    {
        if (!calisiyor)
        {
            if (Keyboard.current != null && Keyboard.current.f9Key.wasPressedThisFrame)
            {
                calisiyor = true;
                sira = 0;
                sureler.Clear();
                satirlar.Clear();
                SceneManager.LoadScene(OyunSahnesi);
            }
            return;
        }

        if (!denemeBasladi)
        {
            return;
        }

        float gecen = Time.time - baslangicZamani;
        yazi.text = "TOZCU TESTİ  deneme " + (sira + 1) + "/" + Baslangiclar.Length + "  süre " + gecen.ToString("0.0") + " sn";

        bool ulasti = tozcu.HedefPanel != null && tozcu.HedefPanel.ElektrikAcikMi;
        if (ulasti)
        {
            sureler.Add(gecen);
            satirlar.Add((sira + 1) + ". (" + Baslangiclar[sira].x + ", " + Baslangiclar[sira].y + ")  başarılı  " + gecen.ToString("0.0") + " sn");
            SonrakiDeneme();
        }
        else if (gecen > ZamanAsimi)
        {
            satirlar.Add((sira + 1) + ". (" + Baslangiclar[sira].x + ", " + Baslangiclar[sira].y + ")  başarısız (60 sn içinde ulaşamadı)");
            SonrakiDeneme();
        }
    }

    private void SonrakiDeneme()
    {
        denemeBasladi = false;
        sira++;

        if (sira < Baslangiclar.Length)
        {
            SceneManager.LoadScene(OyunSahnesi);
            return;
        }

        calisiyor = false;
        Raporla();
    }

    private void Raporla()
    {
        float toplam = 0f;
        foreach (float s in sureler)
        {
            toplam += s;
        }
        float ortalama = sureler.Count > 0 ? toplam / sureler.Count : 0f;

        string ozet = "TOZCU TESTİ BİTTİ\nBaşarı: " + sureler.Count + "/" + Baslangiclar.Length
            + "   Ortalama süre: " + ortalama.ToString("0.0") + " sn";
        string detay = ozet + "\n\n" + string.Join("\n", satirlar);

        yazi.text = detay;
        Debug.Log(detay);

        string dosya = Path.Combine(Application.persistentDataPath, "tozcu_testi.txt");
        File.WriteAllText(dosya, detay);
        Debug.Log("Sonuc dosyasi: " + dosya);
    }
}
