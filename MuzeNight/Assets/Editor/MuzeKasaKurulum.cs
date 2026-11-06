using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

// Tools → Muze → Kasa Kapisi ve ARGUS Kur menusu.
public static class MuzeKasaKurulum
{
    private const string CelikYolu = "Assets/Materials/KasaCelik.mat";
    private const string IcYolu = "Assets/Materials/KasaIci.mat";

    [MenuItem("Tools/Muze/Kasa Kapisi ve ARGUS Kur")]
    public static void Kur()
    {
        KasaKapisiniKur();
        ArgusGizlemeyiKur();

        AssetDatabase.SaveAssets();
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        Debug.Log("Kasa kapisi ve ARGUS gorunurlugu kuruldu. Sahneyi kaydet (Ctrl+S).");
    }

    private static void KasaKapisiniKur()
    {
        GameObject kasaObj = GameObject.Find("Kasa");
        if (kasaObj == null)
        {
            Debug.LogWarning("Kasa bulunamadi.");
            return;
        }

        Material celik = MateryalHazirla(CelikYolu, new Color(0.2f, 0.21f, 0.23f), 0.55f, 0.6f, Color.black);
        Material ic = MateryalHazirla(IcYolu, new Color(0.9f, 0.7f, 0.2f), 0.5f, 0.3f, new Color(1.35f, 0.98f, 0.23f));

        Renderer kasaRenderer = kasaObj.GetComponent<Renderer>();
        Undo.RecordObject(kasaRenderer, "Kasa boya");
        kasaRenderer.sharedMaterial = celik;

        // Kapi, kasanin terminale bakan yuzunde (-X yuzu). Olculer kasanin kendisinden hesaplanir.
        Vector3 merkez = kasaObj.transform.position;
        Vector3 olcek = kasaObj.transform.lossyScale;
        float onYuzX = merkez.x - olcek.x / 2f;
        float yariGenislik = olcek.z / 2f;

        // Kapi kapaliyken arkasinda gizli kalan, acilinca gorunen parlak ic yuzey
        if (GameObject.Find("KasaIci") == null)
        {
            GameObject icYuzey = GameObject.CreatePrimitive(PrimitiveType.Cube);
            Undo.RegisterCreatedObjectUndo(icYuzey, "Kasa ici");
            icYuzey.name = "KasaIci";
            icYuzey.transform.position = new Vector3(onYuzX - 0.005f, merkez.y, merkez.z);
            icYuzey.transform.localScale = new Vector3(0.01f, olcek.y * 0.9f, olcek.z * 0.9f);
            icYuzey.GetComponent<Renderer>().sharedMaterial = ic;
            Object.DestroyImmediate(icYuzey.GetComponent<Collider>());
        }

        // Menteşe kasanin disinda ayri bir obje.
        GameObject mentese = GameObject.Find("KasaKapiMenteseli");
        if (mentese == null)
        {
            mentese = new GameObject("KasaKapiMenteseli");
            Undo.RegisterCreatedObjectUndo(mentese, "Kasa mentese");
            mentese.transform.position = new Vector3(onYuzX - 0.06f, merkez.y, merkez.z + yariGenislik);

            GameObject kapi = GameObject.CreatePrimitive(PrimitiveType.Cube);
            Undo.RegisterCreatedObjectUndo(kapi, "Kasa kapisi");
            kapi.name = "KasaKapisi";
            kapi.transform.SetParent(mentese.transform, false);
            kapi.transform.localPosition = new Vector3(0f, 0f, -yariGenislik);
            kapi.transform.localScale = new Vector3(0.08f, olcek.y * 0.95f, olcek.z * 0.97f);
            kapi.GetComponent<Renderer>().sharedMaterial = celik;

            // Kapi donerken oyuncuyu itmesin
            Object.DestroyImmediate(kapi.GetComponent<Collider>());
        }

        Kasa kasa = kasaObj.GetComponent<Kasa>();
        if (kasa != null)
        {
            SerializedObject ayar = new SerializedObject(kasa);
            ayar.FindProperty("kapiMenteseli").objectReferenceValue = mentese.transform;

            // Kasa acilinca govdeyi camgobegine boyamasin, celik kalsin
            ayar.FindProperty("gosterge").objectReferenceValue = null;
            ayar.ApplyModifiedProperties();
        }
    }

    private static void ArgusGizlemeyiKur()
    {
        GameObject yonetici = GameObject.Find("TerminalManager");
        GameObject panelObj = GameObject.Find("SigortaPaneli");
        if (yonetici == null || panelObj == null)
        {
            Debug.LogWarning("TerminalManager ya da SigortaPaneli bulunamadi.");
            return;
        }

        ArgusGorunurluk gorunurluk = yonetici.GetComponent<ArgusGorunurluk>();
        if (gorunurluk == null)
        {
            gorunurluk = Undo.AddComponent<ArgusGorunurluk>(yonetici);
        }

        // Ikinci panel acilana kadar gizlenecekler: terminal modeli, isigi ve yazi arayuzu
        string[] adlar = { "ARGUS_terminal", "argus", "InputField (TMP)", "cikti" };
        List<GameObject> bulunanlar = new List<GameObject>();
        foreach (string ad in adlar)
        {
            GameObject obj = GameObject.Find(ad);
            if (obj != null)
            {
                bulunanlar.Add(obj);
            }
            else
            {
                Debug.LogWarning("Gizlenecek obje bulunamadi: " + ad);
            }
        }

        SerializedObject ayar = new SerializedObject(gorunurluk);
        ayar.FindProperty("tetikleyiciPanel").objectReferenceValue = panelObj.GetComponent<PowerPanel>();

        SerializedProperty liste = ayar.FindProperty("gizlenecekler");
        liste.arraySize = bulunanlar.Count;
        for (int i = 0; i < bulunanlar.Count; i++)
        {
            liste.GetArrayElementAtIndex(i).objectReferenceValue = bulunanlar[i];
        }

        ayar.ApplyModifiedProperties();
    }

    private static Material MateryalHazirla(string yol, Color renk, float parlaklik, float metalik, Color isima)
    {
        if (!AssetDatabase.IsValidFolder("Assets/Materials"))
        {
            AssetDatabase.CreateFolder("Assets", "Materials");
        }

        Material mevcut = AssetDatabase.LoadAssetAtPath<Material>(yol);
        if (mevcut != null)
        {
            return mevcut;
        }

        Material yeni = new Material(Shader.Find("Standard"));
        yeni.color = renk;
        yeni.SetFloat("_Glossiness", parlaklik);
        yeni.SetFloat("_Metallic", metalik);

        if (isima != Color.black)
        {
            yeni.EnableKeyword("_EMISSION");
            yeni.SetColor("_EmissionColor", isima);
        }

        AssetDatabase.CreateAsset(yeni, yol);
        return yeni;
    }
}
