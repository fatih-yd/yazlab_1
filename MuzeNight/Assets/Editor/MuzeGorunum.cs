using System.IO;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;

// Tools → Muze → Gorunumu Guzellestir menusu.
// Gece atmosferi (karanlik arka plan, sis, hafif ay isigi), tavan, tavan lambalari,
// duvarlara tablolar, kaideler ve kasaya spot isik ekler. Yeni nesnelerin hepsi "Dekor"
// adli tek bir nesnenin altina girer, tekrar calistirinca eskisi silinip yenisi yapilir.
// Dekorlarin carpisticisi yoktur, boylece Tozcu'nun sensorunu ve oyuncuyu etkilemezler.
public static class MuzeGorunum
{
    private const string DekorAdi = "Dekor";

    [MenuItem("Tools/Muze/Gorunumu Guzellestir")]
    public static void GorunumuGuzellestir()
    {
        Atmosfer();

        GameObject eski = GameObject.Find(DekorAdi);
        if (eski != null)
        {
            Undo.DestroyObjectImmediate(eski);
        }

        GameObject kok = new GameObject(DekorAdi);
        Undo.RegisterCreatedObjectUndo(kok, "Dekor");

        Tavan(kok.transform);
        TavanLambalari(kok.transform);
        Tablolar(kok.transform);
        DuvarSuslemeleri(kok.transform);
        Halilar(kok.transform);
        Tabelalar(kok.transform);
        Kaideler(kok.transform);
        KasaIsigi(kok.transform);
        TerminaliParlat();
        TozcuyuRobotYap();
        MuzeYanSalonlar.Kur(kok.transform);

        AssetDatabase.SaveAssets();
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        Debug.Log("Gorunum guzellestirildi. Beğenmezsen Ctrl+Z ile geri al, beğendiysen sahneyi kaydet (Ctrl+S).");
    }

    // Karanlik arka plan, sis ve cok hafif ay isigi
    private static void Atmosfer()
    {
        Camera kamera = Camera.main;
        if (kamera != null)
        {
            Undo.RecordObject(kamera, "Kamera");
            kamera.clearFlags = CameraClearFlags.SolidColor;
            kamera.backgroundColor = new Color(0.02f, 0.02f, 0.035f);
        }

        RenderSettings.skybox = null;
        RenderSettings.ambientMode = AmbientMode.Flat;
        RenderSettings.ambientLight = new Color(0.09f, 0.10f, 0.15f);
        RenderSettings.fog = true;
        RenderSettings.fogMode = FogMode.ExponentialSquared;
        RenderSettings.fogColor = new Color(0.03f, 0.04f, 0.07f);
        RenderSettings.fogDensity = 0.025f;

        Light[] isiklar = Object.FindObjectsByType<Light>(FindObjectsSortMode.None);
        foreach (Light isik in isiklar)
        {
            if (isik.type == LightType.Directional)
            {
                Undo.RecordObject(isik, "Ay isigi");
                isik.intensity = 0.15f;
                isik.color = new Color(0.55f, 0.65f, 1f);
            }
        }
    }

    // Gokyuzu gorunmesin diye odanin ustu kapali
    private static void Tavan(Transform ana)
    {
        Material tavan = Materyal("DekorTavan", new Color(0.05f, 0.05f, 0.07f), 0.1f, Color.black);
        GameObject g = Nesne("Tavan", PrimitiveType.Cube, ana, new Vector3(0f, 3.15f, 0f), new Vector3(31f, 0.3f, 21f), tavan);
        // Tavan golge atmasin, yoksa ay isigi hic iceri girmez
        g.GetComponent<Renderer>().shadowCastingMode = ShadowCastingMode.Off;
    }

    private static void TavanLambalari(Transform ana)
    {
        Material lamba = Materyal("DekorLamba", new Color(0.9f, 0.85f, 0.7f), 0.1f, new Color(1f, 0.85f, 0.6f) * 1.5f);
        float[] xler = { -9f, 6f };
        float[] zler = { -5f, 5f };

        foreach (float x in xler)
        {
            foreach (float z in zler)
            {
                Nesne("TavanLambasi", PrimitiveType.Cube, ana, new Vector3(x, 2.95f, z), new Vector3(1f, 0.05f, 0.4f), lamba);

                GameObject isikNesnesi = new GameObject("TavanIsigi");
                isikNesnesi.transform.SetParent(ana, false);
                isikNesnesi.transform.localPosition = new Vector3(x, 2.7f, z);
                Light isik = isikNesnesi.AddComponent<Light>();
                isik.type = LightType.Point;
                isik.color = new Color(1f, 0.85f, 0.65f);
                isik.intensity = 0.7f;
                isik.range = 9f;
            }
        }
    }

    // Duvarlara cerceveli tablolar
    private static void Tablolar(Transform ana)
    {
        Material cerceve = Materyal("DekorCerceve", new Color(0.35f, 0.25f, 0.1f), 0.5f, Color.black);

        Color[] renkler =
        {
            new Color(0.55f, 0.12f, 0.12f),
            new Color(0.12f, 0.2f, 0.5f),
            new Color(0.12f, 0.4f, 0.2f),
            new Color(0.7f, 0.5f, 0.15f),
            new Color(0.1f, 0.4f, 0.45f)
        };

        // Kuzey duvari (z = +10) ve guney duvari (z = -10).
        // Ara duvarlara (x = -6) ve bariyerlere (x = 0) denk gelmeyecek yerler secildi.
        // Gercek tablolar kamu mali (telifsiz) eserler, Wikimedia Commons'tan alindi.
        // Dosya adi bos olan yerlere soyut resim uretilir.
        // Inci Kupeli Kiz yan salona (Heykel Galerisi) tasindi, yerinde galerinin kapisi var
        float[] kuzeyX = { -13.3f, -3f, 4f, 9f, 13f };
        string[] kuzeyDosya = { "yildizli_gece", "kaplumbaga", "buyuk_dalga", "monalisa", "" };
        string[] kuzeyBilgi =
        {
            "Yıldızlı Gece\nVincent van Gogh, 1889",
            "Kaplumbağa Terbiyecisi\nOsman Hamdi Bey, 1906",
            "Kanagawa'da Büyük Dalga\nKatsushika Hokusai, 1831",
            "Mona Lisa\nLeonardo da Vinci, 1503",
            "Gece Kompozisyonu\nBilinmeyen sanatçı"
        };

        float[] guneyX = { -11f, -3f, 2.9f, 8.9f, 12.5f };
        string[] guneyDosya = { "ciglik", "opucuk", "venusun_dogusu", "gece_devriyesi", "" };
        string[] guneyBilgi =
        {
            "Çığlık\nEdvard Munch, 1893",
            "Öpücük\nGustav Klimt, 1908",
            "Venüs'ün Doğuşu\nSandro Botticelli, 1485",
            "Gece Devriyesi\nRembrandt, 1642",
            "Ufuk\nBilinmeyen sanatçı"
        };

        Material lamba = Materyal("DekorTabloLambasi", new Color(1f, 0.9f, 0.7f), 0.2f, new Color(1f, 0.85f, 0.6f) * 1.2f);
        Material plakaMat = Materyal("DekorPlaka", new Color(0.08f, 0.07f, 0.06f), 0.4f, Color.black);
        int no = 0;

        for (int i = 0; i < kuzeyX.Length; i++)
        {
            TabloEkle(ana, cerceve, lamba, plakaMat, no, kuzeyDosya[i], kuzeyBilgi[i], renkler[i % renkler.Length], renkler[(i + 3) % renkler.Length], new Vector3(kuzeyX[i], 1.85f, 10f), "Tablo_Kuzey_" + i);
            no++;
        }

        for (int i = 0; i < guneyX.Length; i++)
        {
            TabloEkle(ana, cerceve, lamba, plakaMat, no, guneyDosya[i], guneyBilgi[i], renkler[(i + 2) % renkler.Length], renkler[(i + 4) % renkler.Length], new Vector3(guneyX[i], 1.85f, -10f), "Tablo_Guney_" + i);
            no++;
        }
    }

    // konum: tablonun duvar yuzeyindeki orta noktasi
    internal static void TabloEkle(Transform ana, Material cerceve, Material lamba, Material plakaMat, int no, string dosya, string bilgi, Color renk1, Color renk2, Vector3 konum, string ad)
    {
        Texture2D doku = null;
        if (dosya != "")
        {
            doku = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Materials/Tablolar/" + dosya + ".jpg");
        }
        if (doku == null)
        {
            doku = TabloDokusu(no, renk1, renk2);
        }

        // Tablonun boyutu resmin en-boy oranina gore: en fazla 2.1 genis, 1.45 yuksek
        float oran = (float)doku.width / doku.height;
        float yukseklik = 1.45f;
        float genislik = yukseklik * oran;
        if (genislik > 2.1f)
        {
            genislik = 2.1f;
            yukseklik = genislik / oran;
        }

        // Kuzey duvarindaki tablolar odaya (-z), guneydekiler +z yonune bakar
        float iceYon = konum.z > 0f ? -1f : 1f;
        Quaternion donus = iceYon < 0f ? Quaternion.identity : Quaternion.Euler(0f, 180f, 0f);

        Nesne(ad + "_Cerceve", PrimitiveType.Cube, ana, new Vector3(konum.x, konum.y, konum.z + iceYon * 0.04f), new Vector3(genislik + 0.2f, yukseklik + 0.2f, 0.08f), cerceve);

        // Resim biraz parlak olsun ki karanlikta secilsin
        Material resim = Materyal("DekorTablo_" + no, Color.white, 0.2f, Color.white * 0.3f);
        resim.mainTexture = doku;
        resim.SetTexture("_EmissionMap", doku);
        EditorUtility.SetDirty(resim);

        GameObject resimNesnesi = Nesne(ad + "_Resim", PrimitiveType.Quad, ana, new Vector3(konum.x, konum.y, konum.z + iceYon * 0.085f), new Vector3(genislik, yukseklik, 1f), resim);
        resimNesnesi.transform.localRotation = donus;

        // Tablonun ustunde kucuk bir lamba
        float lambaY = Mathf.Min(konum.y + yukseklik / 2f + 0.2f, 2.72f);
        Nesne(ad + "_Lamba", PrimitiveType.Cube, ana, new Vector3(konum.x, lambaY, konum.z + iceYon * 0.08f), new Vector3(0.9f, 0.06f, 0.14f), lamba);

        // Altta lambrinin ustunde isim plakasi
        Vector3 plakaKonum = new Vector3(konum.x, 0.82f, konum.z + iceYon * 0.06f);
        Nesne(ad + "_Plaka", PrimitiveType.Cube, ana, plakaKonum, new Vector3(1.3f, 0.3f, 0.03f), plakaMat);

        GameObject yaziNesnesi = new GameObject(ad + "_PlakaYazi");
        yaziNesnesi.transform.SetParent(ana, false);
        yaziNesnesi.transform.localPosition = new Vector3(konum.x, 0.82f, konum.z + iceYon * 0.08f);
        yaziNesnesi.transform.localRotation = donus;
        TextMeshPro yazi = yaziNesnesi.AddComponent<TextMeshPro>();
        yazi.text = bilgi;
        yazi.fontSize = 1f;
        yazi.alignment = TextAlignmentOptions.Center;
        yazi.color = new Color(0.95f, 0.85f, 0.6f);
        yazi.rectTransform.sizeDelta = new Vector2(1.25f, 0.28f);
    }

    // Iki renkli gecis, ufuk cizgisi ve birkac daireden olusan soyut resim (bir kez uretilir)
    private static Texture2D TabloDokusu(int no, Color renk1, Color renk2)
    {
        string yol = "Assets/Materials/Tablo_" + no + ".png";
        Texture2D mevcut = AssetDatabase.LoadAssetAtPath<Texture2D>(yol);
        if (mevcut != null)
        {
            return mevcut;
        }

        const int en = 128;
        const int boy = 96;
        Texture2D tex = new Texture2D(en, boy, TextureFormat.RGB24, false);
        System.Random rastgele = new System.Random(no * 7 + 1);

        int daireSayisi = 2 + rastgele.Next(3);
        int[] dx = new int[daireSayisi];
        int[] dy = new int[daireSayisi];
        int[] dr = new int[daireSayisi];
        Color[] drenk = new Color[daireSayisi];
        for (int i = 0; i < daireSayisi; i++)
        {
            dx[i] = rastgele.Next(en);
            dy[i] = 20 + rastgele.Next(boy - 20);
            dr[i] = 6 + rastgele.Next(20);
            drenk[i] = Color.Lerp(renk2, Color.white, 0.3f + (float)rastgele.NextDouble() * 0.4f);
        }

        int ufuk = boy / 3 + rastgele.Next(boy / 4);

        for (int py = 0; py < boy; py++)
        {
            for (int px = 0; px < en; px++)
            {
                Color c = Color.Lerp(renk1, renk2, (float)py / boy);
                if (py < ufuk)
                {
                    c = Color.Lerp(renk2, Color.black, 0.5f);
                }

                for (int i = 0; i < daireSayisi; i++)
                {
                    int ax = px - dx[i];
                    int ay = py - dy[i];
                    if (ax * ax + ay * ay < dr[i] * dr[i])
                    {
                        c = drenk[i];
                    }
                }

                tex.SetPixel(px, py, c);
            }
        }

        tex.Apply();
        File.WriteAllBytes(yol, tex.EncodeToPNG());
        AssetDatabase.ImportAsset(yol);
        return AssetDatabase.LoadAssetAtPath<Texture2D>(yol);
    }

    // Duvarlara lambri (alt ahsap kaplama), ust serit, korniş ve mermer sutunlar
    private static void DuvarSuslemeleri(Transform ana)
    {
        Material lambri = Materyal("DekorLambri", new Color(0.2f, 0.12f, 0.07f), 0.4f, Color.black);
        Material serit = Materyal("DekorAltinSerit", new Color(0.75f, 0.58f, 0.25f), 0.7f, new Color(0.3f, 0.22f, 0.08f));
        Material mermer = Materyal("DekorMermer", new Color(0.72f, 0.7f, 0.65f), 0.5f, Color.black);

        // Dis duvarlar: yuzeyin konumu, odanin icine dogru yon, uzunluk.
        // Kuzeyde (x -11.6..-9.4) ve guneyde (x 4.9..7.1) yan salon kapilari var, oralar bos birakilir.
        Bantlar(ana, new Vector3(-13.3f, 0f, 10f), new Vector3(0f, 0f, -1f), 3.4f, true, lambri, serit, mermer);
        Bantlar(ana, new Vector3(2.8f, 0f, 10f), new Vector3(0f, 0f, -1f), 24.4f, true, lambri, serit, mermer);
        Bantlar(ana, new Vector3(-5.05f, 0f, -10f), new Vector3(0f, 0f, 1f), 19.9f, true, lambri, serit, mermer);
        Bantlar(ana, new Vector3(11.05f, 0f, -10f), new Vector3(0f, 0f, 1f), 7.9f, true, lambri, serit, mermer);
        Bantlar(ana, new Vector3(-15f, 0f, 0f), new Vector3(1f, 0f, 0f), 20f, true, lambri, serit, mermer);
        Bantlar(ana, new Vector3(15f, 0f, 0f), new Vector3(-1f, 0f, 0f), 20f, true, lambri, serit, mermer);

        // Ara duvarlar (x = -6), iki yuzu de. Kapi bosluguna (z -1..1) dokunulmaz.
        Bantlar(ana, new Vector3(-6.25f, 0f, -5.5f), new Vector3(-1f, 0f, 0f), 9f, false, lambri, serit, mermer);
        Bantlar(ana, new Vector3(-5.75f, 0f, -5.5f), new Vector3(1f, 0f, 0f), 9f, false, lambri, serit, mermer);
        Bantlar(ana, new Vector3(-6.25f, 0f, 5.5f), new Vector3(-1f, 0f, 0f), 9f, false, lambri, serit, mermer);
        Bantlar(ana, new Vector3(-5.75f, 0f, 5.5f), new Vector3(1f, 0f, 0f), 9f, false, lambri, serit, mermer);

        // Sutunlar tablolarin arasina denk gelecek sekilde. Kapilarin iki yaninda da birer sutun var.
        float[] kuzeyX = { -11.9f, -9.1f, -4.7f, 1.8f, 6.5f, 11f, 14.5f };
        float[] guneyX = { -14.5f, -8.5f, -4.7f, 1.3f, 4.6f, 7.4f, 10.25f, 14.5f };
        foreach (float x in kuzeyX)
        {
            Sutun(ana, new Vector3(x, 0f, 10f), new Vector3(0f, 0f, -1f), mermer);
        }
        foreach (float x in guneyX)
        {
            Sutun(ana, new Vector3(x, 0f, -10f), new Vector3(0f, 0f, 1f), mermer);
        }
        Sutun(ana, new Vector3(-15f, 0f, -6f), new Vector3(1f, 0f, 0f), mermer);
        Sutun(ana, new Vector3(-15f, 0f, 6f), new Vector3(1f, 0f, 0f), mermer);
        Sutun(ana, new Vector3(15f, 0f, -5f), new Vector3(-1f, 0f, 0f), mermer);
        Sutun(ana, new Vector3(15f, 0f, 5f), new Vector3(-1f, 0f, 0f), mermer);
    }

    // Bir duvar yuzune lambri, ust serit ve (istenirse) korniş ekler
    private static void Bantlar(Transform ana, Vector3 yuzey, Vector3 iceYon, float uzunluk, bool kornisVar, Material lambri, Material serit, Material mermer)
    {
        Bant(ana, "Lambri", yuzey, iceYon, uzunluk, 0.5f, 1f, 0.04f, lambri);
        Bant(ana, "Serit", yuzey, iceYon, uzunluk, 1.02f, 0.05f, 0.08f, serit);
        if (kornisVar)
        {
            Bant(ana, "Kornis", yuzey, iceYon, uzunluk, 2.88f, 0.18f, 0.12f, mermer);
        }
    }

    private static void Bant(Transform ana, string ad, Vector3 yuzey, Vector3 iceYon, float uzunluk, float y, float yukseklik, float derinlik, Material m)
    {
        Vector3 merkez = yuzey + iceYon * (derinlik / 2f);
        merkez.y = y;
        bool xBoyunca = Mathf.Abs(iceYon.z) > 0.5f;
        Vector3 olcek = xBoyunca ? new Vector3(uzunluk, yukseklik, derinlik) : new Vector3(derinlik, yukseklik, uzunluk);
        Nesne(ad, PrimitiveType.Cube, ana, merkez, olcek, m);
    }

    // Duvara yaslanmis yarim sutun: govde, alt taban ve ust baslik
    private static void Sutun(Transform ana, Vector3 yuzey, Vector3 iceYon, Material mermer)
    {
        bool xBoyunca = Mathf.Abs(iceYon.z) > 0.5f;
        Vector3 govde = xBoyunca ? new Vector3(0.5f, 2.9f, 0.25f) : new Vector3(0.25f, 2.9f, 0.5f);
        Vector3 baslik = xBoyunca ? new Vector3(0.7f, 0.22f, 0.35f) : new Vector3(0.35f, 0.22f, 0.7f);

        Vector3 merkez = yuzey + iceYon * 0.125f;
        Nesne("Sutun", PrimitiveType.Cube, ana, new Vector3(merkez.x, 1.45f, merkez.z), govde, mermer);

        Vector3 tabanMerkez = yuzey + iceYon * 0.175f;
        Nesne("SutunTaban", PrimitiveType.Cube, ana, new Vector3(tabanMerkez.x, 0.11f, tabanMerkez.z), baslik, mermer);
        Nesne("SutunBaslik", PrimitiveType.Cube, ana, new Vector3(tabanMerkez.x, 2.75f, tabanMerkez.z), baslik, mermer);
    }

    // Yolu gosteren kirmizi halilar (altin kenarli)
    private static void Halilar(Transform ana)
    {
        Material kenar = Materyal("DekorHaliKenar", new Color(0.6f, 0.45f, 0.15f), 0.2f, Color.black);
        Material hali = Materyal("DekorHali", new Color(0.45f, 0.05f, 0.07f), 0.1f, Color.black);

        // Baslangic odasindaki hali kuzeydeki galeri kapisina kadar uzanir
        HaliEkle(ana, new Vector3(-10.5f, 0f, 0.75f), new Vector2(2.2f, 18.5f), kenar, hali);
        HaliEkle(ana, new Vector3(-3.2f, 0f, 0f), new Vector2(5f, 2.2f), kenar, hali);
        HaliEkle(ana, new Vector3(6f, 0f, 0f), new Vector2(10f, 2.2f), kenar, hali);
        // Kasa salonundan guneydeki Misir Salonu kapisina
        HaliEkle(ana, new Vector3(6f, 0f, -5.55f), new Vector2(2.2f, 8.9f), kenar, hali);
    }

    private static void HaliEkle(Transform ana, Vector3 merkez, Vector2 boyut, Material kenar, Material hali)
    {
        Nesne("HaliKenar", PrimitiveType.Cube, ana, new Vector3(merkez.x, 0.01f, merkez.z), new Vector3(boyut.x, 0.02f, boyut.y), kenar);
        Nesne("Hali", PrimitiveType.Cube, ana, new Vector3(merkez.x, 0.015f, merkez.z), new Vector3(boyut.x - 0.2f, 0.02f, boyut.y - 0.2f), hali);
    }

    // Salon tabelalari
    private static void Tabelalar(Transform ana)
    {
        // Bati duvari (soldan bakan oyuncuya donuk) ve dogu duvari (kasanin arkasi)
        Tabela(ana, "ANTİK ESERLER SALONU", new Vector3(-14.85f, 2.35f, -1f), -90f);
        Tabela(ana, "KASA - GÜVENLİK BÖLGESİ", new Vector3(14.85f, 2.35f, 0f), 90f);
    }

    internal static void Tabela(Transform ana, string yazi, Vector3 konum, float yDonus)
    {
        GameObject g = new GameObject("Tabela");
        g.transform.SetParent(ana, false);
        g.transform.localPosition = konum;
        g.transform.localRotation = Quaternion.Euler(0f, yDonus, 0f);

        TextMeshPro t = g.AddComponent<TextMeshPro>();
        t.text = yazi;
        t.fontSize = 3f;
        t.alignment = TextAlignmentOptions.Center;
        t.color = new Color(0.95f, 0.8f, 0.45f);
        t.rectTransform.sizeDelta = new Vector2(8f, 1f);
    }

    // Duvar diplerinde parlayan eser duran kaideler
    private static void Kaideler(Transform ana)
    {
        Material tas = Materyal("DekorKaide", new Color(0.55f, 0.55f, 0.58f), 0.3f, Color.black);

        Material altin = Materyal("DekorEserAltin", new Color(1f, 0.7f, 0.2f), 0.8f, new Color(1f, 0.65f, 0.15f) * 0.8f);
        Material mavi = Materyal("DekorEserMavi", new Color(0.2f, 0.8f, 1f), 0.8f, new Color(0.15f, 0.7f, 1f) * 0.8f);
        Material mor = Materyal("DekorEserMor", new Color(0.8f, 0.3f, 1f), 0.8f, new Color(0.7f, 0.2f, 1f) * 0.8f);
        Material[] eserler = { altin, mavi, mor };

        // Ilk uc kaide baslangic odasinda (x -15..-6). Bunlarin eserleri toplanabilir, plakalarinda ipucu var.
        Vector3[] konumlar =
        {
            new Vector3(-13.3f, 0f, 8.4f), new Vector3(-8.5f, 0f, 8.4f), new Vector3(-14f, 0f, 3f),
            new Vector3(-5f, 0f, 8.4f), new Vector3(3f, 0f, 8.4f), new Vector3(7f, 0f, 8.4f),
            new Vector3(3f, 0f, -8.4f), new Vector3(8f, 0f, -8.4f), new Vector3(12f, 0f, -8.4f)
        };

        string[] eserAdlari = { "Altın Küre", "Mavi Tılsım", "Mor Kristal" };
        string[] plakalar =
        {
            "Elektrik kesildiğinde kasadaki eserler nem ve sıcaklık değişiminden zarar görür. Böyle bir durumda kasayı kontrol etmek gerekir.",
            "Kasaya yalnızca müze müdürü, güvenlik şefi, teknik sorumlu ve koruma görevlileri erişebilir.",
            "ARGUS tehdit ve hakaretten etkilenmez. Yetkisini ve eserler için acil nedenini birlikte anlatan kişiyi dinler."
        };

        PrimitiveType[] sekiller = { PrimitiveType.Sphere, PrimitiveType.Cylinder, PrimitiveType.Capsule };

        for (int i = 0; i < konumlar.Length; i++)
        {
            Vector3 k = konumlar[i];
            Nesne("Kaide_" + i, PrimitiveType.Cube, ana, new Vector3(k.x, 0.5f, k.z), new Vector3(0.7f, 1f, 0.7f), tas);
            GameObject eser = Nesne("Eser_" + i, sekiller[i % sekiller.Length], ana, new Vector3(k.x, 1.35f, k.z), new Vector3(0.4f, 0.4f, 0.4f), eserler[i % eserler.Length]);

            if (i < eserAdlari.Length)
            {
                eser.AddComponent<Eser>().Ayarla(eserAdlari[i], plakalar[i]);
            }
        }

        // Plaka okuma gorevini yoneten nesne
        GameObject gorev = new GameObject("GorevYoneticisi");
        gorev.transform.SetParent(ana, false);
        gorev.AddComponent<GorevPaneli>();
    }

    // Kasanin ustunden sicak bir spot isik
    private static void KasaIsigi(Transform ana)
    {
        GameObject kasa = GameObject.Find("Kasa");
        Vector3 hedef = kasa != null ? kasa.transform.position : new Vector3(12f, 0.5f, 0f);

        GameObject g = new GameObject("KasaSpot");
        g.transform.SetParent(ana, false);
        g.transform.position = new Vector3(hedef.x, 2.9f, hedef.z);
        g.transform.rotation = Quaternion.Euler(90f, 0f, 0f);

        Light isik = g.AddComponent<Light>();
        isik.type = LightType.Spot;
        isik.color = new Color(1f, 0.8f, 0.5f);
        isik.intensity = 3f;
        isik.range = 6f;
        isik.spotAngle = 60f;
    }

    // Terminal govdesi karanlikta parlasin
    private static void TerminaliParlat()
    {
        GameObject terminal = GameObject.Find("ARGUS_terminal");
        if (terminal == null)
        {
            return;
        }

        Renderer r = terminal.GetComponent<Renderer>();
        if (r == null)
        {
            return;
        }

        Undo.RecordObject(r, "Terminal");
        r.sharedMaterial = Materyal("DekorTerminal", new Color(0.02f, 0.06f, 0.07f), 0.6f, new Color(0.1f, 0.8f, 1f) * 0.9f);
    }

    // Tozcu'nun kapsul gorunumunu gizleyip ustune basit bir robot govdesi takar.
    // Kapsulun carpistiricisi ve NavMeshAgent'i oldugu gibi kalir, sadece gorunum degisir.
    private static void TozcuyuRobotYap()
    {
        GameObject tozcu = GameObject.Find("Tozcu");
        if (tozcu == null)
        {
            return;
        }

        Transform eskiGovde = tozcu.transform.Find("RobotGovde");
        if (eskiGovde != null)
        {
            Undo.DestroyObjectImmediate(eskiGovde.gameObject);
        }

        MeshRenderer kapsul = tozcu.GetComponent<MeshRenderer>();
        if (kapsul != null)
        {
            Undo.RecordObject(kapsul, "Tozcu kapsul");
            kapsul.enabled = false;
        }

        GameObject govde = new GameObject("RobotGovde");
        Undo.RegisterCreatedObjectUndo(govde, "Robot govde");
        govde.transform.SetParent(tozcu.transform, false);

        Material sari = Materyal("RobotSari", new Color(0.95f, 0.65f, 0.1f), 0.6f, Color.black);
        Material metal = Materyal("RobotMetal", new Color(0.45f, 0.47f, 0.5f), 0.7f, Color.black);
        Material koyu = Materyal("RobotKoyu", new Color(0.08f, 0.08f, 0.1f), 0.3f, Color.black);
        Material goz = Materyal("RobotGoz", new Color(0.2f, 0.9f, 1f), 0.8f, new Color(0.2f, 0.9f, 1f) * 2f);
        Material lamba = Materyal("RobotLamba", new Color(1f, 0.2f, 0.2f), 0.8f, new Color(1f, 0.15f, 0.15f) * 2f);

        Transform g = govde.transform;
        Nesne("Govde", PrimitiveType.Cube, g, new Vector3(0f, -0.2f, 0f), new Vector3(0.8f, 0.9f, 0.6f), sari);
        Nesne("Kafa", PrimitiveType.Cube, g, new Vector3(0f, 0.5f, 0f), new Vector3(0.55f, 0.4f, 0.45f), metal);
        Nesne("GozSol", PrimitiveType.Sphere, g, new Vector3(-0.13f, 0.52f, 0.23f), new Vector3(0.12f, 0.12f, 0.06f), goz);
        Nesne("GozSag", PrimitiveType.Sphere, g, new Vector3(0.13f, 0.52f, 0.23f), new Vector3(0.12f, 0.12f, 0.06f), goz);
        Nesne("Anten", PrimitiveType.Cylinder, g, new Vector3(0f, 0.8f, 0f), new Vector3(0.04f, 0.1f, 0.04f), metal);
        Nesne("AntenLamba", PrimitiveType.Sphere, g, new Vector3(0f, 0.95f, 0f), new Vector3(0.1f, 0.1f, 0.1f), lamba);
        Nesne("KolSol", PrimitiveType.Cube, g, new Vector3(-0.5f, -0.15f, 0f), new Vector3(0.15f, 0.55f, 0.15f), metal);
        Nesne("KolSag", PrimitiveType.Cube, g, new Vector3(0.5f, -0.15f, 0f), new Vector3(0.15f, 0.55f, 0.15f), metal);

        GameObject tekerSol = Nesne("TekerSol", PrimitiveType.Cylinder, g, new Vector3(-0.42f, -0.75f, 0f), new Vector3(0.45f, 0.06f, 0.45f), koyu);
        GameObject tekerSag = Nesne("TekerSag", PrimitiveType.Cylinder, g, new Vector3(0.42f, -0.75f, 0f), new Vector3(0.45f, 0.06f, 0.45f), koyu);
        tekerSol.transform.localRotation = Quaternion.Euler(0f, 0f, 90f);
        tekerSag.transform.localRotation = Quaternion.Euler(0f, 0f, 90f);
    }

    // Collider'siz primitive nesne olusturur (Tozcu'nun sensoru ve oyuncu takilmasin)
    internal static GameObject Nesne(string ad, PrimitiveType tip, Transform ana, Vector3 konum, Vector3 olcek, Material materyal)
    {
        GameObject g = GameObject.CreatePrimitive(tip);
        g.name = ad;
        g.transform.SetParent(ana, false);
        g.transform.localPosition = konum;
        g.transform.localScale = olcek;
        g.GetComponent<Renderer>().sharedMaterial = materyal;

        Collider c = g.GetComponent<Collider>();
        if (c != null)
        {
            Object.DestroyImmediate(c);
        }
        return g;
    }

    // Varsa mevcut materyali gunceller, yoksa yeni uretir
    internal static Material Materyal(string ad, Color renk, float parlaklik, Color isima)
    {
        string yol = "Assets/Materials/" + ad + ".mat";
        Material m = AssetDatabase.LoadAssetAtPath<Material>(yol);
        if (m == null)
        {
            m = new Material(Shader.Find("Standard"));
            AssetDatabase.CreateAsset(m, yol);
        }

        m.color = renk;
        m.SetFloat("_Glossiness", parlaklik);

        if (isima.maxColorComponent > 0f)
        {
            m.EnableKeyword("_EMISSION");
            m.SetColor("_EmissionColor", isima);
        }
        else
        {
            m.DisableKeyword("_EMISSION");
            m.SetColor("_EmissionColor", Color.black);
        }

        EditorUtility.SetDirty(m);
        return m;
    }
}
