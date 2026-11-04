using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.AI;

// Tools → Muze → Hareketli Bariyerleri Kur menusu.
// Ortadaki Engel duvarinin kuzeyindeki ve guneyindeki bosluklara, sirayla acilip kapanan
// iki bariyer kurar. Biri kapaliyken digeri acik oldugu icin yol hicbir zaman tamamen kapanmaz.
// Ctrl+Z ile geri alinabilir, tekrar calistirilirsa var olanlari cogaltmaz.
public static class MuzeBariyerKurulum
{
    private const string MateryalYolu = "Assets/Materials/BariyerUyari.mat";
    private const float Yukseklik = 3f;

    [MenuItem("Tools/Muze/Hareketli Bariyerleri Kur")]
    public static void Kur()
    {
        GameObject engel = GameObject.Find("Engel");
        GameObject zemin = GameObject.Find("Zemin");
        if (engel == null || zemin == null)
        {
            Debug.LogWarning("Engel ya da Zemin bulunamadi.");
            return;
        }

        Material uyari = MateryalHazirla();

        // Bosluklar: Engel'in uclari ile zeminin kenarlari arasi
        Bounds engelSinir = engel.GetComponent<Renderer>().bounds;
        Bounds zeminSinir = zemin.GetComponent<Renderer>().bounds;
        float x = engelSinir.center.x;
        float kalinlik = engelSinir.size.x;

        float kuzeyBas = engelSinir.max.z;
        float kuzeySon = zeminSinir.max.z;
        float guneyBas = zeminSinir.min.z;
        float guneySon = engelSinir.min.z;

        // Kuzey kapali, guney acik baslar; ayni zamanlamayla sirayla yer degistirirler
        BariyerEkle("Bariyer_Kuzey", x, kuzeyBas, kuzeySon, kalinlik, uyari, true);
        BariyerEkle("Bariyer_Guney", x, guneyBas, guneySon, kalinlik, uyari, false);

        AssetDatabase.SaveAssets();
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        Debug.Log("Hareketli bariyerler kuruldu. Sahneyi kaydet (Ctrl+S).");
    }

    private static void BariyerEkle(string ad, float x, float zBas, float zSon, float kalinlik, Material materyal, bool kapaliBasla)
    {
        if (GameObject.Find(ad) != null)
        {
            Debug.Log("Zaten var, atlandi: " + ad);
            return;
        }

        float uzunluk = zSon - zBas;
        float merkezZ = (zBas + zSon) / 2f;

        GameObject bariyer = GameObject.CreatePrimitive(PrimitiveType.Cube);
        Undo.RegisterCreatedObjectUndo(bariyer, "Bariyer ekle");
        bariyer.name = ad;

        // Sahnede kapali (yukarida) konumda durur, oyun baslayinca script asagi indirebilir
        bariyer.transform.position = new Vector3(x, Yukseklik / 2f, merkezZ);
        bariyer.transform.localScale = new Vector3(kalinlik, Yukseklik, uzunluk);
        bariyer.GetComponent<Renderer>().sharedMaterial = materyal;

        // Diger duvarlar gibi: NavMesh'e bake edilmez, carve ile engel olur.
        // Hareket ederken de carve guncellensin ki Tozcu hemen tepki versin.
        GameObjectUtility.SetStaticEditorFlags(bariyer, 0);
        NavMeshObstacle navEngeli = bariyer.AddComponent<NavMeshObstacle>();
        navEngeli.shape = NavMeshObstacleShape.Box;
        navEngeli.carving = true;
        navEngeli.carveOnlyStationary = false;

        HareketliBariyer hareket = bariyer.AddComponent<HareketliBariyer>();
        SerializedObject ayar = new SerializedObject(hareket);
        ayar.FindProperty("kapaliBasla").boolValue = kapaliBasla;
        ayar.ApplyModifiedProperties();
    }

    private static Material MateryalHazirla()
    {
        if (!AssetDatabase.IsValidFolder("Assets/Materials"))
        {
            AssetDatabase.CreateFolder("Assets", "Materials");
        }

        Material mevcut = AssetDatabase.LoadAssetAtPath<Material>(MateryalYolu);
        if (mevcut != null)
        {
            return mevcut;
        }

        // Karanlik sahnede fark edilsin diye hafif isiyan uyari sarisi
        Material yeni = new Material(Shader.Find("Standard"));
        yeni.color = new Color(0.95f, 0.6f, 0.05f);
        yeni.SetFloat("_Glossiness", 0.4f);
        yeni.EnableKeyword("_EMISSION");
        yeni.SetColor("_EmissionColor", new Color(0.45f, 0.25f, 0.02f));
        AssetDatabase.CreateAsset(yeni, MateryalYolu);
        return yeni;
    }
}
