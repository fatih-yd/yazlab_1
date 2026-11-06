using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.AI;

// Tools → Muze → Duvarlari ve Zemini Hazirla menusu.
public static class MuzeDuvarlari
{
    private const string DokuYolu = "Assets/Materials/KoyuDoku.png";
    private const string ZeminMateryalYolu = "Assets/Materials/KoyuZemin.mat";
    private const string DuvarMateryalYolu = "Assets/Materials/KoyuDuvar.mat";
    private const string KapiMateryalYolu = "Assets/Materials/KapiMetal.mat";

    // Zemin: X -15..15, Z -10..10. Cevre duvarlari kenarin hemen disinda, koseler kapali.
    private const float DuvarYuksekligi = 3f;
    private const float DuvarKalinligi = 0.5f;

    [MenuItem("Tools/Muze/Duvarlari ve Zemini Hazirla")]
    public static void DuvarlariVeZeminiHazirla()
    {
        Texture2D doku = DokuHazirla();

        // Ayni doku, farkli tekrar: zemin genis, duvarlar uzun ve alcak
        Material zemin = MateryalHazirla(ZeminMateryalYolu, doku, new Vector2(15f, 10f), Color.white, 0.25f, 0f);
        Material duvar = MateryalHazirla(DuvarMateryalYolu, doku, new Vector2(10f, 1f), Color.white, 0.25f, 0f);
        Material kapiMetal = MateryalHazirla(KapiMateryalYolu, null, Vector2.one, new Color(0.36f, 0.22f, 0.12f), 0.35f, 0.1f);

        MevcutObjeyiBoya("Zemin", zemin);
        MevcutObjeyiBoya("left_wall", duvar);
        MevcutObjeyiBoya("right_wall", duvar);
        MevcutObjeyiBoya("Engel", duvar);

        // Gecit kapisi duvardan ayirt edilsin diye kahverengi ahsap gorunumunde
        MevcutObjeyiBoya("door", kapiMetal);

        // Alanin dort kenarina cevre duvarlari
        float y = DuvarYuksekligi / 2f;
        YeniDuvarEkle("Duvar_Kuzey", new Vector3(0f, y, 10.25f), new Vector3(31f, DuvarYuksekligi, DuvarKalinligi), duvar);
        YeniDuvarEkle("Duvar_Guney", new Vector3(0f, y, -10.25f), new Vector3(31f, DuvarYuksekligi, DuvarKalinligi), duvar);
        YeniDuvarEkle("Duvar_Dogu", new Vector3(15.25f, y, 0f), new Vector3(DuvarKalinligi, DuvarYuksekligi, 21f), duvar);
        YeniDuvarEkle("Duvar_Bati", new Vector3(-15.25f, y, 0f), new Vector3(DuvarKalinligi, DuvarYuksekligi, 21f), duvar);

        AssetDatabase.SaveAssets();
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        Debug.Log("Zemin ve duvarlar hazirlandi. Sahneyi kaydetmeyi unutma (Ctrl+S).");
    }

    // Koyu, hafif puruzlu, ince derzli tas karo dokusu uretir (bir kez, sonra hazir olani kullanir)
    private static Texture2D DokuHazirla()
    {
        KlasoruHazirla();

        Texture2D mevcut = AssetDatabase.LoadAssetAtPath<Texture2D>(DokuYolu);
        if (mevcut != null)
        {
            return mevcut;
        }

        const int boyut = 256;
        const int karo = 128;
        const int derzKalinligi = 2;

        Texture2D tex = new Texture2D(boyut, boyut, TextureFormat.RGB24, false);
        System.Random rastgele = new System.Random(42);

        for (int py = 0; py < boyut; py++)
        {
            for (int px = 0; px < boyut; px++)
            {
                bool derz = (px % karo) < derzKalinligi || (py % karo) < derzKalinligi;

                // Karolar arasinda hafif ton farki, karo icinde ince puruz
                float karoTonu = ((px / karo + py / karo) % 2 == 0) ? 0f : 0.012f;
                float puruz = (float)rastgele.NextDouble() * 0.03f - 0.015f;
                float deger = derz ? 0.03f : 0.075f + karoTonu + puruz;

                tex.SetPixel(px, py, new Color(deger, deger, deger * 1.08f));
            }
        }

        tex.Apply();
        File.WriteAllBytes(DokuYolu, tex.EncodeToPNG());
        Object.DestroyImmediate(tex);

        AssetDatabase.ImportAsset(DokuYolu);
        TextureImporter ayar = (TextureImporter)AssetImporter.GetAtPath(DokuYolu);
        ayar.wrapMode = TextureWrapMode.Repeat;
        ayar.SaveAndReimport();

        return AssetDatabase.LoadAssetAtPath<Texture2D>(DokuYolu);
    }

    private static Material MateryalHazirla(string yol, Texture2D doku, Vector2 tekrar, Color renk, float parlaklik, float metalik)
    {
        KlasoruHazirla();

        Material mevcut = AssetDatabase.LoadAssetAtPath<Material>(yol);
        if (mevcut != null)
        {
            return mevcut;
        }

        Material yeni = new Material(Shader.Find("Standard"));
        yeni.color = renk;
        yeni.mainTexture = doku;
        yeni.mainTextureScale = tekrar;
        yeni.SetFloat("_Glossiness", parlaklik);
        yeni.SetFloat("_Metallic", metalik);
        AssetDatabase.CreateAsset(yeni, yol);
        return yeni;
    }

    private static void KlasoruHazirla()
    {
        if (!AssetDatabase.IsValidFolder("Assets/Materials"))
        {
            AssetDatabase.CreateFolder("Assets", "Materials");
        }
    }

    private static void MevcutObjeyiBoya(string objeAdi, Material materyal)
    {
        GameObject obj = GameObject.Find(objeAdi);
        if (obj == null)
        {
            Debug.LogWarning("Bulunamadi: " + objeAdi);
            return;
        }

        Renderer rend = obj.GetComponent<Renderer>();
        if (rend != null)
        {
            Undo.RecordObject(rend, "Doku uygula");
            rend.sharedMaterial = materyal;
        }
    }

    private static void YeniDuvarEkle(string ad, Vector3 konum, Vector3 boyut, Material materyal)
    {
        if (GameObject.Find(ad) != null)
        {
            Debug.Log("Zaten var, atlandi: " + ad);
            return;
        }

        GameObject duvar = GameObject.CreatePrimitive(PrimitiveType.Cube);
        Undo.RegisterCreatedObjectUndo(duvar, "Duvar ekle");
        duvar.name = ad;
        duvar.transform.position = konum;
        duvar.transform.localScale = boyut;
        duvar.GetComponent<Renderer>().sharedMaterial = materyal;

        // Diger duvarlarla ayni yontem: NavMesh'e bake edilmez, carve ile engel olur
        GameObjectUtility.SetStaticEditorFlags(duvar, 0);
        NavMeshObstacle engel = duvar.AddComponent<NavMeshObstacle>();
        engel.shape = NavMeshObstacleShape.Box;
        engel.carving = true;
    }
}
