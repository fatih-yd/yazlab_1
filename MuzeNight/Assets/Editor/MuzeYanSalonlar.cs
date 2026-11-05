using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

// "Gorunumu Guzellestir" menusunun parcasi. Kuzey ve guney dis duvarlarina kapi acar,
// arkalarina gezilebilir iki salon kurar:
//   - Heykel Galerisi (baslangic odasinin kuzeyi): bustler, heykel, banklar, tablolar
//   - Antik Misir Salonu (kasa salonunun guneyi): lahit, piramitler, dikilitaslar, vitrinler
// Her salonda okunabilen bir hikaye notu var (Eser bileseni, E ile okunur).
// Duvarlar ve zeminler carpistiricili (oyuncu iceri girip duvarlardan gecemesin),
// dekor parcalari carpistiricisiz.
public static class MuzeYanSalonlar
{
    public static void Kur(Transform ana)
    {
        GameObject kok = new GameObject("YanSalonlar");
        kok.transform.SetParent(ana, false);
        Transform k = kok.transform;

        Material duvar = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/KoyuDuvar.mat");
        if (duvar == null)
        {
            duvar = MuzeGorunum.Materyal("DekorDuvar", new Color(0.12f, 0.12f, 0.14f), 0.2f, Color.black);
        }
        Material zemin = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/KoyuZemin.mat");
        if (zemin == null)
        {
            zemin = duvar;
        }

        AnaDuvarlariAc(k, duvar);
        HeykelGalerisi(k, duvar, zemin);
        MisirSalonu(k, duvar);
    }

    // Eski tek parca kuzey/guney duvarlari gizlenir, yerine kapi bosluklu parcalar konur
    private static void AnaDuvarlariAc(Transform ana, Material duvar)
    {
        DuvariGizle("Duvar_Kuzey");
        DuvariGizle("Duvar_Guney");

        // Kuzey: kapi x -11.6..-9.4
        Duvar(ana, "Duvar_Kuzey_Bati", new Vector3(-13.55f, 1.5f, 10.25f), new Vector3(3.9f, 3f, 0.5f), duvar);
        Duvar(ana, "Duvar_Kuzey_Dogu", new Vector3(3.05f, 1.5f, 10.25f), new Vector3(24.9f, 3f, 0.5f), duvar);
        Duvar(ana, "Kapi_Kuzey_Ust", new Vector3(-10.5f, 2.7f, 10.25f), new Vector3(2.2f, 0.6f, 0.5f), duvar);

        // Guney: kapi x 4.9..7.1
        Duvar(ana, "Duvar_Guney_Bati", new Vector3(-5.3f, 1.5f, -10.25f), new Vector3(20.4f, 3f, 0.5f), duvar);
        Duvar(ana, "Duvar_Guney_Dogu", new Vector3(11.3f, 1.5f, -10.25f), new Vector3(8.4f, 3f, 0.5f), duvar);
        Duvar(ana, "Kapi_Guney_Ust", new Vector3(6f, 2.7f, -10.25f), new Vector3(2.2f, 0.6f, 0.5f), duvar);

        // Kapilarin ustunde salon adlari (ana salondan bakana donuk)
        // Kapi tabelalari sutunlarin onunde ve kapi genisligine sigacak boyda
        MuzeGorunum.Tabela(ana, "HEYKEL GALERİSİ", new Vector3(-10.5f, 2.7f, 9.6f), 0f, 1.5f);
        MuzeGorunum.Tabela(ana, "ANTİK MISIR SALONU", new Vector3(6f, 2.7f, -9.6f), 180f, 1.3f);
    }

    // ---------------------------------------------------------------- Heykel Galerisi
    // Ic alan: x -15..-6, z 10.5..17
    private static void HeykelGalerisi(Transform ana, Material duvar, Material zemin)
    {
        Transform g = AltNesne(ana, "HeykelGalerisi");

        Duvar(g, "ArkaDuvar", new Vector3(-10.5f, 1.5f, 17.25f), new Vector3(10f, 3f, 0.5f), duvar);
        Duvar(g, "BatiDuvari", new Vector3(-15.25f, 1.5f, 13.75f), new Vector3(0.5f, 3f, 7.5f), duvar);
        Duvar(g, "DoguDuvari", new Vector3(-5.75f, 1.5f, 13.75f), new Vector3(0.5f, 3f, 7.5f), duvar);
        Duvar(g, "Zemin", new Vector3(-10.5f, -0.1f, 13.75f), new Vector3(10f, 0.2f, 7.5f), zemin);
        Tavan(g, new Vector3(-10.5f, 3.15f, 13.75f), new Vector3(10f, 0.3f, 7.5f));

        Isik(g, new Vector3(-12.5f, 2.6f, 13.5f), new Color(1f, 0.85f, 0.65f), 1.1f, 7f);
        Isik(g, new Vector3(-8.5f, 2.6f, 13.5f), new Color(1f, 0.85f, 0.65f), 1.1f, 7f);

        Material mermer = MuzeGorunum.Materyal("DekorHeykelMermer", new Color(0.85f, 0.83f, 0.78f), 0.5f, new Color(0.08f, 0.08f, 0.07f));
        Material ahsap = MuzeGorunum.Materyal("DekorAhsap", new Color(0.3f, 0.18f, 0.09f), 0.4f, Color.black);

        // Arka duvarda tablolar
        Material cerceve = MuzeGorunum.Materyal("DekorCerceve", new Color(0.35f, 0.25f, 0.1f), 0.5f, Color.black);
        Material lamba = MuzeGorunum.Materyal("DekorTabloLambasi", new Color(1f, 0.9f, 0.7f), 0.2f, new Color(1f, 0.85f, 0.6f) * 1.2f);
        Material plaka = MuzeGorunum.Materyal("DekorPlaka", new Color(0.08f, 0.07f, 0.06f), 0.4f, Color.black);
        MuzeGorunum.TabloEkle(g, cerceve, lamba, plaka, 20, "inci_kupeli_kiz", "İnci Küpeli Kız\nJohannes Vermeer, 1665",
            Color.black, Color.black, new Vector3(-10.5f, 1.85f, 17f), "Galeri_Tablo_Orta");
        MuzeGorunum.TabloEkle(g, cerceve, lamba, plaka, 21, "", "Sabah\nBilinmeyen sanatçı",
            new Color(0.7f, 0.5f, 0.15f), new Color(0.12f, 0.2f, 0.5f), new Vector3(-13.3f, 1.85f, 17f), "Galeri_Tablo_Sol");
        MuzeGorunum.TabloEkle(g, cerceve, lamba, plaka, 22, "", "Liman\nBilinmeyen sanatçı",
            new Color(0.1f, 0.4f, 0.45f), new Color(0.55f, 0.12f, 0.12f), new Vector3(-7.7f, 1.85f, 17f), "Galeri_Tablo_Sag");

        // Yan duvarlar boyunca bustler
        Bust(g, new Vector3(-14.3f, 0f, 12.2f), mermer);
        Bust(g, new Vector3(-14.3f, 0f, 15f), mermer);
        Bust(g, new Vector3(-6.7f, 0f, 12.2f), mermer);
        Bust(g, new Vector3(-6.7f, 0f, 15f), mermer);

        // Ortada kordonla cevrili buyuk heykel
        Heykel(g, new Vector3(-10.5f, 0f, 14f), mermer);
        Kordon(g, new Vector3(-10.5f, 0f, 14f), 1f, 1f);

        // Banklar ve uzerinde hikaye notu
        Bank(g, new Vector3(-13f, 0f, 11.4f), ahsap);
        Bank(g, new Vector3(-8f, 0f, 11.4f), ahsap);
        Not(g, new Vector3(-13f, 0.52f, 11.4f), "Gece Bekçisinin Notu",
            "Saat 23.40. Elektrikler gitti, ARGUS yedek güçle kasayı kilitledi. Sigorta paneli kasa salonunda, Tozcu yolunu bilir. " +
            "Kasadaki eserler nemden zarar görebilir, müdür sabaha kadar gelemeyecek.");
    }

    // ---------------------------------------------------------------- Antik Misir Salonu
    // Ic alan: x 1..11, z -17..-10.5
    private static void MisirSalonu(Transform ana, Material duvar)
    {
        Transform g = AltNesne(ana, "MisirSalonu");

        Material kumtasi = MuzeGorunum.Materyal("DekorKumtasi", new Color(0.42f, 0.33f, 0.2f), 0.2f, Color.black);
        Duvar(g, "ArkaDuvar", new Vector3(6f, 1.5f, -17.25f), new Vector3(10.5f, 3f, 0.5f), duvar);
        Duvar(g, "BatiDuvari", new Vector3(0.75f, 1.5f, -13.75f), new Vector3(0.5f, 3f, 7.5f), duvar);
        Duvar(g, "DoguDuvari", new Vector3(11.25f, 1.5f, -13.75f), new Vector3(0.5f, 3f, 7.5f), duvar);
        Duvar(g, "Zemin", new Vector3(6f, -0.1f, -13.75f), new Vector3(10.5f, 0.2f, 7.5f), kumtasi);
        Tavan(g, new Vector3(6f, 3.15f, -13.75f), new Vector3(10.5f, 0.3f, 7.5f));

        Isik(g, new Vector3(3.5f, 2.6f, -13.5f), new Color(1f, 0.7f, 0.4f), 1.1f, 7f);
        Isik(g, new Vector3(8.5f, 2.6f, -13.5f), new Color(1f, 0.7f, 0.4f), 1.1f, 7f);

        // Yan duvarlarda hiyeroglif panolari
        Texture2D hiyeroglif = HiyeroglifDokusu();
        Material pano = MuzeGorunum.Materyal("DekorHiyeroglif", Color.white, 0.1f, Color.white * 0.25f);
        pano.mainTexture = hiyeroglif;
        pano.SetTexture("_EmissionMap", hiyeroglif);
        EditorUtility.SetDirty(pano);

        GameObject batiPano = MuzeGorunum.Nesne("HiyeroglifBati", PrimitiveType.Quad, g, new Vector3(1.02f, 1.7f, -13.75f), new Vector3(6f, 1.6f, 1f), pano);
        batiPano.transform.localRotation = Quaternion.Euler(0f, -90f, 0f);
        GameObject doguPano = MuzeGorunum.Nesne("HiyeroglifDogu", PrimitiveType.Quad, g, new Vector3(10.98f, 1.7f, -13.75f), new Vector3(6f, 1.6f, 1f), pano);
        doguPano.transform.localRotation = Quaternion.Euler(0f, 90f, 0f);

        Material altin = MuzeGorunum.Materyal("DekorMisirAltin", new Color(0.95f, 0.7f, 0.2f), 0.85f, new Color(0.35f, 0.22f, 0.05f));
        Material lacivert = MuzeGorunum.Materyal("DekorMisirLacivert", new Color(0.1f, 0.18f, 0.5f), 0.7f, new Color(0.02f, 0.04f, 0.12f));
        Material tas = MuzeGorunum.Materyal("DekorDikilitas", new Color(0.55f, 0.45f, 0.32f), 0.3f, Color.black);
        Material ahsap = MuzeGorunum.Materyal("DekorAhsap", new Color(0.3f, 0.18f, 0.09f), 0.4f, Color.black);

        // Ortada lahit, kordonla cevrili, ustunde spot isik
        Lahit(g, new Vector3(6f, 0f, -13.6f), altin, lacivert);
        Kordon(g, new Vector3(6f, 0f, -13.6f), 1f, 1.5f);
        Spot(g, new Vector3(6f, 2.9f, -13.6f), new Color(1f, 0.8f, 0.5f));

        // Arka koselerde piramit maketleri, arka duvar onunde dikilitaslar
        Piramit(g, new Vector3(2.3f, 0f, -15.7f), 1.5f, 1.1f, tas);
        Piramit(g, new Vector3(9.7f, 0f, -15.7f), 1.5f, 1.1f, tas);
        Dikilitas(g, new Vector3(4.3f, 0f, -16.5f), tas);
        Dikilitas(g, new Vector3(7.7f, 0f, -16.5f), tas);

        // Giris yaninda cam vitrinler
        Vitrin(g, new Vector3(2.2f, 0f, -11.7f), ahsap, altin, lacivert, true);
        Vitrin(g, new Vector3(9.8f, 0f, -11.7f), ahsap, altin, lacivert, false);

        // Kursu uzerinde hikaye notu
        MuzeGorunum.Nesne("Kursu", PrimitiveType.Cube, g, new Vector3(3.6f, 0.5f, -11.3f), new Vector3(0.5f, 1f, 0.4f), ahsap);
        Not(g, new Vector3(3.6f, 1.02f, -11.3f), "Küratörün Notu",
            "Firavun maskesi ve Mısır koleksiyonunun en değerli parçaları bu gece güvenlik için kasaya taşındı. " +
            "Kasayı yalnızca yetkili bir görevli, eserler tehlikedeyse ARGUS'a açtırabilir.");
    }

    // ---------------------------------------------------------------- Parcalar

    private static void Bust(Transform ana, Vector3 taban, Material mermer)
    {
        MuzeGorunum.Nesne("BustKaide", PrimitiveType.Cylinder, ana, taban + new Vector3(0f, 0.55f, 0f), new Vector3(0.5f, 0.55f, 0.5f), mermer);
        MuzeGorunum.Nesne("BustTabla", PrimitiveType.Cube, ana, taban + new Vector3(0f, 1.14f, 0f), new Vector3(0.6f, 0.08f, 0.6f), mermer);
        MuzeGorunum.Nesne("BustGogus", PrimitiveType.Sphere, ana, taban + new Vector3(0f, 1.38f, 0f), new Vector3(0.55f, 0.4f, 0.32f), mermer);
        MuzeGorunum.Nesne("BustBoyun", PrimitiveType.Cylinder, ana, taban + new Vector3(0f, 1.62f, 0f), new Vector3(0.14f, 0.08f, 0.14f), mermer);
        MuzeGorunum.Nesne("BustKafa", PrimitiveType.Sphere, ana, taban + new Vector3(0f, 1.84f, 0f), new Vector3(0.28f, 0.34f, 0.3f), mermer);
    }

    // Kaba hatlarla ayakta duran bir heykel
    private static void Heykel(Transform ana, Vector3 taban, Material mermer)
    {
        MuzeGorunum.Nesne("HeykelKaide", PrimitiveType.Cube, ana, taban + new Vector3(0f, 0.25f, 0f), new Vector3(1.1f, 0.5f, 1.1f), mermer);
        MuzeGorunum.Nesne("BacakSol", PrimitiveType.Capsule, ana, taban + new Vector3(-0.12f, 0.95f, 0f), new Vector3(0.17f, 0.42f, 0.17f), mermer);
        MuzeGorunum.Nesne("BacakSag", PrimitiveType.Capsule, ana, taban + new Vector3(0.12f, 0.95f, 0f), new Vector3(0.17f, 0.42f, 0.17f), mermer);
        MuzeGorunum.Nesne("Govde", PrimitiveType.Capsule, ana, taban + new Vector3(0f, 1.6f, 0f), new Vector3(0.42f, 0.38f, 0.28f), mermer);
        MuzeGorunum.Nesne("Kafa", PrimitiveType.Sphere, ana, taban + new Vector3(0f, 2.12f, 0f), new Vector3(0.24f, 0.28f, 0.26f), mermer);

        GameObject kolSol = MuzeGorunum.Nesne("KolSol", PrimitiveType.Capsule, ana, taban + new Vector3(-0.3f, 1.7f, 0f), new Vector3(0.12f, 0.33f, 0.12f), mermer);
        kolSol.transform.localRotation = Quaternion.Euler(0f, 0f, -25f);
        GameObject kolSag = MuzeGorunum.Nesne("KolSag", PrimitiveType.Capsule, ana, taban + new Vector3(0.36f, 1.95f, 0f), new Vector3(0.12f, 0.33f, 0.12f), mermer);
        kolSag.transform.localRotation = Quaternion.Euler(0f, 0f, 35f);
    }

    // Dort direk ve aralarinda kirmizi kadife kordon
    private static void Kordon(Transform ana, Vector3 merkez, float yariX, float yariZ)
    {
        Material direk = MuzeGorunum.Materyal("DekorKordonDirek", new Color(0.8f, 0.62f, 0.25f), 0.8f, Color.black);
        Material kadife = MuzeGorunum.Materyal("DekorKadife", new Color(0.5f, 0.04f, 0.06f), 0.3f, Color.black);

        Vector3[] koseler =
        {
            merkez + new Vector3(-yariX, 0f, -yariZ), merkez + new Vector3(yariX, 0f, -yariZ),
            merkez + new Vector3(yariX, 0f, yariZ), merkez + new Vector3(-yariX, 0f, yariZ)
        };

        foreach (Vector3 kose in koseler)
        {
            MuzeGorunum.Nesne("KordonDirek", PrimitiveType.Cylinder, ana, kose + new Vector3(0f, 0.45f, 0f), new Vector3(0.06f, 0.45f, 0.06f), direk);
            MuzeGorunum.Nesne("KordonTepe", PrimitiveType.Sphere, ana, kose + new Vector3(0f, 0.92f, 0f), new Vector3(0.1f, 0.1f, 0.1f), direk);
        }

        for (int i = 0; i < 4; i++)
        {
            Vector3 a = koseler[i];
            Vector3 b = koseler[(i + 1) % 4];
            Vector3 orta = (a + b) / 2f + new Vector3(0f, 0.78f, 0f);
            float uzunluk = Vector3.Distance(a, b);
            bool xBoyunca = Mathf.Abs(a.z - b.z) < 0.01f;

            GameObject ip = MuzeGorunum.Nesne("Kordon", PrimitiveType.Cylinder, ana, orta, new Vector3(0.03f, uzunluk / 2f, 0.03f), kadife);
            ip.transform.localRotation = xBoyunca ? Quaternion.Euler(0f, 0f, 90f) : Quaternion.Euler(90f, 0f, 0f);
        }
    }

    private static void Bank(Transform ana, Vector3 taban, Material ahsap)
    {
        MuzeGorunum.Nesne("BankOturak", PrimitiveType.Cube, ana, taban + new Vector3(0f, 0.45f, 0f), new Vector3(1.5f, 0.08f, 0.45f), ahsap);
        MuzeGorunum.Nesne("BankAyak", PrimitiveType.Cube, ana, taban + new Vector3(-0.62f, 0.21f, 0f), new Vector3(0.08f, 0.42f, 0.4f), ahsap);
        MuzeGorunum.Nesne("BankAyak", PrimitiveType.Cube, ana, taban + new Vector3(0.62f, 0.21f, 0f), new Vector3(0.08f, 0.42f, 0.4f), ahsap);
    }

    // Okunabilen hikaye notu (gorev panelindeki sayaca eklenir)
    private static void Not(Transform ana, Vector3 konum, string baslik, string metin)
    {
        Material kagit = MuzeGorunum.Materyal("DekorKagit", new Color(0.9f, 0.86f, 0.72f), 0.1f, new Color(0.3f, 0.28f, 0.22f));
        GameObject not = MuzeGorunum.Nesne("Not_" + baslik, PrimitiveType.Cube, ana, konum, new Vector3(0.3f, 0.01f, 0.22f), kagit);
        not.AddComponent<Eser>().Ayarla(baslik, metin);
    }

    private static void Lahit(Transform ana, Vector3 taban, Material altin, Material lacivert)
    {
        MuzeGorunum.Nesne("LahitKaide", PrimitiveType.Cube, ana, taban + new Vector3(0f, 0.15f, 0f), new Vector3(1.2f, 0.3f, 2.3f), lacivert);
        MuzeGorunum.Nesne("LahitGovde", PrimitiveType.Cube, ana, taban + new Vector3(0f, 0.55f, 0f), new Vector3(0.85f, 0.5f, 2f), altin);
        MuzeGorunum.Nesne("LahitKapak", PrimitiveType.Capsule, ana, taban + new Vector3(0f, 0.85f, 0f), new Vector3(0.8f, 1f, 0.35f), altin)
            .transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
        MuzeGorunum.Nesne("LahitYuz", PrimitiveType.Sphere, ana, taban + new Vector3(0f, 1.02f, 0.75f), new Vector3(0.38f, 0.22f, 0.42f), altin);
        MuzeGorunum.Nesne("LahitSerit", PrimitiveType.Cube, ana, taban + new Vector3(0f, 0.95f, 0.2f), new Vector3(0.82f, 0.05f, 0.12f), lacivert);
        MuzeGorunum.Nesne("LahitSerit", PrimitiveType.Cube, ana, taban + new Vector3(0f, 0.95f, -0.3f), new Vector3(0.82f, 0.05f, 0.12f), lacivert);
    }

    private static void Piramit(Transform ana, Vector3 taban, float genislik, float yukseklik, Material tas)
    {
        MuzeGorunum.Nesne("PiramitAltlik", PrimitiveType.Cube, ana, taban + new Vector3(0f, 0.1f, 0f), new Vector3(genislik + 0.3f, 0.2f, genislik + 0.3f), tas);

        GameObject p = new GameObject("Piramit", typeof(MeshFilter), typeof(MeshRenderer));
        p.transform.SetParent(ana, false);
        p.transform.localPosition = taban + new Vector3(0f, 0.2f, 0f);
        p.transform.localScale = new Vector3(genislik, yukseklik, genislik);
        p.GetComponent<MeshFilter>().sharedMesh = PiramitMesh();
        p.GetComponent<MeshRenderer>().sharedMaterial = tas;
    }

    private static void Dikilitas(Transform ana, Vector3 taban, Material tas)
    {
        MuzeGorunum.Nesne("DikilitasTaban", PrimitiveType.Cube, ana, taban + new Vector3(0f, 0.15f, 0f), new Vector3(0.6f, 0.3f, 0.6f), tas);
        MuzeGorunum.Nesne("DikilitasGovde", PrimitiveType.Cube, ana, taban + new Vector3(0f, 1.4f, 0f), new Vector3(0.32f, 2.2f, 0.32f), tas);

        GameObject tepe = new GameObject("DikilitasTepe", typeof(MeshFilter), typeof(MeshRenderer));
        tepe.transform.SetParent(ana, false);
        tepe.transform.localPosition = taban + new Vector3(0f, 2.5f, 0f);
        tepe.transform.localScale = new Vector3(0.32f, 0.3f, 0.32f);
        tepe.GetComponent<MeshFilter>().sharedMesh = PiramitMesh();
        tepe.GetComponent<MeshRenderer>().sharedMaterial = MuzeGorunum.Materyal("DekorMisirAltin", new Color(0.95f, 0.7f, 0.2f), 0.85f, new Color(0.35f, 0.22f, 0.05f));
    }

    // Ahsap masa, ustunde cam kutu ve icinde eserler
    private static void Vitrin(Transform ana, Vector3 taban, Material ahsap, Material altin, Material lacivert, bool maske)
    {
        MuzeGorunum.Nesne("VitrinMasa", PrimitiveType.Cube, ana, taban + new Vector3(0f, 0.45f, 0f), new Vector3(1f, 0.9f, 0.6f), ahsap);
        MuzeGorunum.Nesne("VitrinCam", PrimitiveType.Cube, ana, taban + new Vector3(0f, 1.2f, 0f), new Vector3(1f, 0.6f, 0.6f), CamMateryali());

        if (maske)
        {
            MuzeGorunum.Nesne("Maske", PrimitiveType.Sphere, ana, taban + new Vector3(0f, 1.12f, 0f), new Vector3(0.22f, 0.3f, 0.18f), altin);
            MuzeGorunum.Nesne("MaskeSerit", PrimitiveType.Cube, ana, taban + new Vector3(0f, 1.18f, 0f), new Vector3(0.24f, 0.04f, 0.19f), lacivert);
        }
        else
        {
            for (int i = 0; i < 3; i++)
            {
                Vector3 kavanoz = taban + new Vector3(-0.3f + i * 0.3f, 1.02f, 0f);
                MuzeGorunum.Nesne("Kavanoz", PrimitiveType.Cylinder, ana, kavanoz, new Vector3(0.12f, 0.1f, 0.12f), i == 1 ? lacivert : altin);
                MuzeGorunum.Nesne("KavanozKapak", PrimitiveType.Sphere, ana, kavanoz + new Vector3(0f, 0.12f, 0f), new Vector3(0.1f, 0.08f, 0.1f), altin);
            }
        }
    }

    // ---------------------------------------------------------------- Yardimcilar

    private static Transform AltNesne(Transform ana, string ad)
    {
        GameObject g = new GameObject(ad);
        g.transform.SetParent(ana, false);
        return g.transform;
    }

    private static void DuvariGizle(string ad)
    {
        GameObject g = GameObject.Find(ad);
        if (g != null)
        {
            Undo.RecordObject(g, "Duvari gizle");
            g.SetActive(false);
        }
    }

    // Carpistiricisi kalan kutu (duvar, zemin)
    private static GameObject Duvar(Transform ana, string ad, Vector3 konum, Vector3 olcek, Material m)
    {
        GameObject g = GameObject.CreatePrimitive(PrimitiveType.Cube);
        g.name = ad;
        g.transform.SetParent(ana, false);
        g.transform.localPosition = konum;
        g.transform.localScale = olcek;
        g.GetComponent<Renderer>().sharedMaterial = m;
        return g;
    }

    private static void Tavan(Transform ana, Vector3 konum, Vector3 olcek)
    {
        Material tavan = MuzeGorunum.Materyal("DekorTavan", new Color(0.05f, 0.05f, 0.07f), 0.1f, Color.black);
        GameObject g = MuzeGorunum.Nesne("Tavan", PrimitiveType.Cube, ana, konum, olcek, tavan);
        g.GetComponent<Renderer>().shadowCastingMode = ShadowCastingMode.Off;
    }

    private static void Isik(Transform ana, Vector3 konum, Color renk, float siddet, float menzil)
    {
        Material lamba = MuzeGorunum.Materyal("DekorLamba", new Color(0.9f, 0.85f, 0.7f), 0.1f, new Color(1f, 0.85f, 0.6f) * 1.5f);
        MuzeGorunum.Nesne("TavanLambasi", PrimitiveType.Cube, ana, new Vector3(konum.x, 2.95f, konum.z), new Vector3(1f, 0.05f, 0.4f), lamba);

        GameObject g = new GameObject("Isik");
        g.transform.SetParent(ana, false);
        g.transform.localPosition = konum;
        Light isik = g.AddComponent<Light>();
        isik.type = LightType.Point;
        isik.color = renk;
        isik.intensity = siddet;
        isik.range = menzil;
    }

    private static void Spot(Transform ana, Vector3 konum, Color renk)
    {
        GameObject g = new GameObject("Spot");
        g.transform.SetParent(ana, false);
        g.transform.localPosition = konum;
        g.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
        Light isik = g.AddComponent<Light>();
        isik.type = LightType.Spot;
        isik.color = renk;
        isik.intensity = 2.5f;
        isik.range = 5f;
        isik.spotAngle = 55f;
    }

    // Yari saydam cam (Standard shader'in Transparent modu)
    private static Material CamMateryali()
    {
        Material m = MuzeGorunum.Materyal("DekorCam", new Color(0.6f, 0.8f, 0.9f, 0.18f), 0.95f, Color.black);
        m.SetFloat("_Mode", 3f);
        m.SetInt("_SrcBlend", (int)BlendMode.One);
        m.SetInt("_DstBlend", (int)BlendMode.OneMinusSrcAlpha);
        m.SetInt("_ZWrite", 0);
        m.DisableKeyword("_ALPHATEST_ON");
        m.DisableKeyword("_ALPHABLEND_ON");
        m.EnableKeyword("_ALPHAPREMULTIPLY_ON");
        m.renderQueue = 3000;
        EditorUtility.SetDirty(m);
        return m;
    }

    // Tabani 1x1, yuksekligi 1 olan piramit. Iki yuzlu cizilir ki her yonden gorunsun.
    private static Mesh PiramitMesh()
    {
        const string yol = "Assets/Materials/Piramit.asset";
        Mesh mevcut = AssetDatabase.LoadAssetAtPath<Mesh>(yol);
        if (mevcut != null)
        {
            return mevcut;
        }

        Vector3 a = new Vector3(-0.5f, 0f, -0.5f);
        Vector3 b = new Vector3(0.5f, 0f, -0.5f);
        Vector3 c = new Vector3(0.5f, 0f, 0.5f);
        Vector3 d = new Vector3(-0.5f, 0f, 0.5f);
        Vector3 t = new Vector3(0f, 1f, 0f);

        Vector3[][] ucgenler =
        {
            new[] { a, b, t }, new[] { b, c, t }, new[] { c, d, t }, new[] { d, a, t },
            new[] { a, c, b }, new[] { a, d, c }
        };

        Vector3[] noktalar = new Vector3[ucgenler.Length * 6];
        int[] sira = new int[ucgenler.Length * 6];
        int n = 0;
        foreach (Vector3[] u in ucgenler)
        {
            // On yuz ve arka yuz
            noktalar[n] = u[0]; noktalar[n + 1] = u[1]; noktalar[n + 2] = u[2];
            noktalar[n + 3] = u[0]; noktalar[n + 4] = u[2]; noktalar[n + 5] = u[1];
            for (int i = 0; i < 6; i++)
            {
                sira[n + i] = n + i;
            }
            n += 6;
        }

        Mesh mesh = new Mesh();
        mesh.name = "Piramit";
        mesh.vertices = noktalar;
        mesh.triangles = sira;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        AssetDatabase.CreateAsset(mesh, yol);
        return mesh;
    }

    // Kum renginde zemin uzerinde satir satir koyu isaretler (bir kez uretilir)
    private static Texture2D HiyeroglifDokusu()
    {
        const string yol = "Assets/Materials/Hiyeroglif.png";
        Texture2D mevcut = AssetDatabase.LoadAssetAtPath<Texture2D>(yol);
        if (mevcut != null)
        {
            return mevcut;
        }

        const int en = 384;
        const int boy = 96;
        Color kum = new Color(0.72f, 0.6f, 0.4f);
        Color murekkep = new Color(0.22f, 0.15f, 0.1f);
        Color mavi = new Color(0.15f, 0.3f, 0.55f);
        Texture2D tex = new Texture2D(en, boy, TextureFormat.RGB24, false);
        System.Random rastgele = new System.Random(7);

        for (int py = 0; py < boy; py++)
        {
            for (int px = 0; px < en; px++)
            {
                tex.SetPixel(px, py, kum);
            }
        }

        // 3 satir, her satirda 12x12 hucreli rastgele isaretler
        for (int satir = 0; satir < 3; satir++)
        {
            int y0 = 8 + satir * 30;
            for (int hucre = 0; hucre < en / 16; hucre++)
            {
                int x0 = 4 + hucre * 16;
                Color renk = rastgele.Next(6) == 0 ? mavi : murekkep;
                int tip = rastgele.Next(4);
                for (int yy = 0; yy < 14; yy++)
                {
                    for (int xx = 0; xx < 12; xx++)
                    {
                        bool dolu;
                        if (tip == 0) dolu = (xx - 6) * (xx - 6) + (yy - 7) * (yy - 7) < 25;   // daire
                        else if (tip == 1) dolu = xx == 5 || xx == 6 || yy == 12 || yy == 13;   // ters T
                        else if (tip == 2) dolu = yy > 9 || (xx > 3 && xx < 8);                // ayak ve govde
                        else dolu = (yy + xx) % 7 < 2;                                          // capraz cizgiler
                        if (dolu)
                        {
                            tex.SetPixel(x0 + xx, y0 + yy, renk);
                        }
                    }
                }
            }
            // Satir arasi cizgi
            for (int px = 0; px < en; px++)
            {
                tex.SetPixel(px, y0 - 4, murekkep);
            }
        }

        tex.Apply();
        File.WriteAllBytes(yol, tex.EncodeToPNG());
        AssetDatabase.ImportAsset(yol);
        return AssetDatabase.LoadAssetAtPath<Texture2D>(yol);
    }
}
