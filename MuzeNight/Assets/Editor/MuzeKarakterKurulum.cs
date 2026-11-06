using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

// Tools → Muze → ARGUS Karakterini Kur menusu.
public static class MuzeKarakterKurulum
{
    private const string PaketKlasoru = "Assets/ArtStore3D/Insurgent Lite(LITE)/Prefab/";

    // Poz dosyasinin icinde yan yana uc karakter var (Insurgent_Pose0, 1, 2); yalnizca biri kullanilir.
    private const int SecilenPoz = 0;

    private const string KarakterYolu = PaketKlasoru + "Insurgent_Pose.prefab";
    private const string SilahYolu = PaketKlasoru + "Ak47.prefab";
    private const string KarakterAdi = "ARGUS_Gorevli";
    private const string SilahAdi = "ARGUS_Silah";

    // Oyuncunun goz hizasi 1.6 m, boyu ~1.8 m; ARGUS iki kati
    private const float HedefBoy = 3.6f;

    [MenuItem("Tools/Muze/ARGUS Karakterini Kur")]
    public static void Kur()
    {
        GameObject terminal = GameObject.Find("ARGUS_terminal");
        GameObject karakterPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(KarakterYolu);
        GameObject silahPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(SilahYolu);
        if (terminal == null || karakterPrefab == null || silahPrefab == null)
        {
            Debug.LogWarning("ARGUS_terminal, karakter ya da silah prefab'i bulunamadi.");
            return;
        }

        // Eski karakter (ve silahi) varsa silinir, yenisi temiz kurulur
        GameObject eski = GameObject.Find(KarakterAdi);
        if (eski != null)
        {
            Undo.DestroyObjectImmediate(eski);
        }

        GameObject karakter = (GameObject)PrefabUtility.InstantiatePrefab(karakterPrefab);
        Undo.RegisterCreatedObjectUndo(karakter, "ARGUS karakteri ekle");
        karakter.name = KarakterAdi;

        // Oyuncuya (-X yonune) bakar
        karakter.transform.rotation = Quaternion.Euler(0f, -90f, 0f);

        // Once tek poz birakilir; boy ve konum yalnizca o karaktere gore hesaplanir
        TekPozBirak(karakter);
        BoyunuAyarla(karakter);

        Vector3 t = terminal.transform.position;
        Yerlestir(karakter, new Vector3(t.x, 0f, t.z));
        CarpismaEkle(karakter);

        // Silah karakter yerlestikten sonra eklenir ki boy hesabini bozmasin
        SilahiVer(karakter, silahPrefab);

        TerminalKupunuGizle(terminal);
        IsigiBasinUstuneAl(t);
        ArgusIleGizle(karakter);

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        Debug.Log("ARGUS karakteri kuruldu. Silahin acisini gerekirse Scene penceresinde ayarla, sonra kaydet (Ctrl+S).");
    }

    // Secilen poz acik kalir, digerleri kapatilir (prefab baglantisi bozulmasin diye silinmez)
    private static void TekPozBirak(GameObject karakter)
    {
        string secilenAd = "Pose" + SecilenPoz;
        bool bulundu = false;

        foreach (Transform parca in karakter.GetComponentsInChildren<Transform>(true))
        {
            // Yalnizca numarali poz objeleri (Pose0, Pose1, Pose2); numarasiz ara objelere dokunulmaz
            if (parca == karakter.transform || !Regex.IsMatch(parca.name, @"Pose\d+$"))
            {
                continue;
            }

            bool secili = parca.name.EndsWith(secilenAd);
            bulundu |= secili;

            Undo.RecordObject(parca.gameObject, "Poz sec");
            parca.gameObject.SetActive(secili);
        }

        if (!bulundu)
        {
            Debug.LogWarning("Secilen poz bulunamadi: " + secilenAd + ". Tum pozlar acik birakildi.");
            foreach (Transform parca in karakter.GetComponentsInChildren<Transform>(true))
            {
                parca.gameObject.SetActive(true);
            }
        }
    }

    // Modelin olcegi bilinmedigi icin gorunur boyuna gore olceklenir
    private static void BoyunuAyarla(GameObject karakter)
    {
        Bounds sinir = Sinirlar(karakter);
        if (sinir.size.y > 0.001f)
        {
            karakter.transform.localScale *= HedefBoy / sinir.size.y;
        }
    }

    // Gorunen karakterin ortasi hedef noktaya, ayaklari zemine (y = 0) gelir
    private static void Yerlestir(GameObject karakter, Vector3 hedef)
    {
        Bounds sinir = Sinirlar(karakter);
        karakter.transform.position += new Vector3(hedef.x - sinir.center.x, -sinir.min.y, hedef.z - sinir.center.z);
    }

    // Oyuncu icinden gecmesin; NavMesh'i etkilemez. Kapsul gorunen karakterin ortasina konur.
    private static void CarpismaEkle(GameObject karakter)
    {
        Bounds sinir = Sinirlar(karakter);
        Transform k = karakter.transform;
        float olcek = k.localScale.y;

        CapsuleCollider govde = karakter.AddComponent<CapsuleCollider>();
        govde.height = HedefBoy / olcek;
        govde.radius = 0.5f / olcek;
        govde.center = k.InverseTransformPoint(new Vector3(sinir.center.x, sinir.min.y + HedefBoy / 2f, sinir.center.z));
    }

    // Poz modelinde kemik olmadigi icin silah elin yaklasik yerine (sag on, gogus hizasi) konur.
    private static void SilahiVer(GameObject karakter, GameObject silahPrefab)
    {
        Bounds karakterSinir = Sinirlar(karakter);
        Vector3 taban = new Vector3(karakterSinir.center.x, karakterSinir.min.y, karakterSinir.center.z);

        GameObject silah = (GameObject)PrefabUtility.InstantiatePrefab(silahPrefab);
        Undo.RegisterCreatedObjectUndo(silah, "ARGUS silahi ekle");
        silah.name = SilahAdi;

        // Uzunlugu karakterin boyuyla orantili olsun (insan boyunda ~0.85 m'lik tufek)
        Bounds sinir = Sinirlar(silah);
        float enUzun = Mathf.Max(sinir.size.x, Mathf.Max(sinir.size.y, sinir.size.z));
        if (enUzun > 0.001f)
        {
            silah.transform.localScale *= (HedefBoy * 0.47f) / enUzun;
        }

        Transform k = karakter.transform;
        silah.transform.rotation = k.rotation;
        silah.transform.position = taban
                                   + Vector3.up * (HedefBoy * 0.55f)
                                   + k.forward * (HedefBoy * 0.12f)
                                   + k.right * (HedefBoy * 0.08f);

        Undo.SetTransformParent(silah.transform, k, "Silahi karaktere bagla");
    }

    // Yalnizca gorunen (aktif) parcalar sayilir; kapatilan pozlar hesaba girmez
    private static Bounds Sinirlar(GameObject obj)
    {
        Renderer[] parcalar = obj.GetComponentsInChildren<Renderer>();
        if (parcalar.Length == 0)
        {
            return new Bounds(obj.transform.position, Vector3.zero);
        }

        Bounds sinir = parcalar[0].bounds;
        foreach (Renderer parca in parcalar)
        {
            sinir.Encapsulate(parca.bounds);
        }

        return sinir;
    }

    // Obje kalir (ARGUS'un gizlenip belirmesi ona bagli), sadece kup gorunumu ve carpismasi kapanir
    private static void TerminalKupunuGizle(GameObject terminal)
    {
        Renderer gorunum = terminal.GetComponent<Renderer>();
        if (gorunum != null)
        {
            Undo.RecordObject(gorunum, "Terminal kupunu gizle");
            gorunum.enabled = false;
        }

        Collider carpisma = terminal.GetComponent<Collider>();
        if (carpisma != null)
        {
            Undo.RecordObject(carpisma, "Terminal carpismasini kapat");
            carpisma.enabled = false;
        }
    }

    // ARGUS'un isigi karakterin govdesinin icinde kalmasin, basinin ustunden aydinlatsin
    private static void IsigiBasinUstuneAl(Vector3 terminalKonumu)
    {
        GameObject isik = GameObject.Find("argus");
        if (isik == null)
        {
            return;
        }

        Undo.RecordObject(isik.transform, "ARGUS isigi");
        isik.transform.position = new Vector3(terminalKonumu.x - 1f, HedefBoy + 0.6f, terminalKonumu.z);
    }

    // ARGUS gorunurluk listesine eklenir; silinmis eski karakterin bos kaydi temizlenir
    private static void ArgusIleGizle(GameObject karakter)
    {
        GameObject yonetici = GameObject.Find("TerminalManager");
        ArgusGorunurluk gorunurluk = yonetici != null ? yonetici.GetComponent<ArgusGorunurluk>() : null;
        if (gorunurluk == null)
        {
            Debug.LogWarning("ArgusGorunurluk bulunamadi. Once 'Kasa Kapisi ve ARGUS Kur' aracini calistir.");
            return;
        }

        SerializedObject ayar = new SerializedObject(gorunurluk);
        SerializedProperty liste = ayar.FindProperty("gizlenecekler");

        List<Object> kalanlar = new List<Object>();
        for (int i = 0; i < liste.arraySize; i++)
        {
            Object eleman = liste.GetArrayElementAtIndex(i).objectReferenceValue;
            if (eleman != null)
            {
                kalanlar.Add(eleman);
            }
        }

        if (!kalanlar.Contains(karakter))
        {
            kalanlar.Add(karakter);
        }

        liste.arraySize = kalanlar.Count;
        for (int i = 0; i < kalanlar.Count; i++)
        {
            liste.GetArrayElementAtIndex(i).objectReferenceValue = kalanlar[i];
        }

        ayar.ApplyModifiedProperties();
    }
}
