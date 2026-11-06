using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

// Tools → Muze → Diyalog Arayuzunu Kur menusu.
public static class MuzeArayuzKurulum
{
    private static readonly Color Amber = new Color(0.95f, 0.62f, 0.12f);
    private static readonly Color Zemin = new Color(0.03f, 0.03f, 0.04f, 0.88f);
    private static readonly Color KutuZemini = new Color(0.11f, 0.11f, 0.13f, 1f);
    private static readonly Color MetinRengi = new Color(0.92f, 0.92f, 0.92f);
    private static readonly Color SoluRenk = new Color(0.55f, 0.55f, 0.55f);

    [MenuItem("Tools/Muze/Diyalog Arayuzunu Kur")]
    public static void Kur()
    {
        GameObject canvasObj = GameObject.Find("Canvas");
        GameObject ciktiObj = GameObject.Find("cikti");
        GameObject girdiObj = GameObject.Find("InputField (TMP)");
        if (canvasObj == null || ciktiObj == null || girdiObj == null)
        {
            Debug.LogWarning("Canvas, cikti ya da InputField (TMP) bulunamadi.");
            return;
        }

        CanvasAyarla(canvasObj);

        RectTransform panel = PaneliHazirla(canvasObj.transform);
        BaslikHazirla(panel);
        CiktiyiYerlestir(ciktiObj, panel);
        GirdiyiYerlestir(girdiObj, panel);
        ArgusIleGizle(panel.gameObject);

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        Debug.Log("Diyalog arayuzu kuruldu. Sahneyi kaydet (Ctrl+S).");
    }

    // Arayuz ekrana sabit olsun ve her cozunurlukte ayni oranda gorunsun
    private static void CanvasAyarla(GameObject canvasObj)
    {
        Canvas canvas = canvasObj.GetComponent<Canvas>();
        Undo.RecordObject(canvas, "Canvas ayarla");
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        CanvasScaler olcekleyici = canvasObj.GetComponent<CanvasScaler>();
        if (olcekleyici == null)
        {
            olcekleyici = Undo.AddComponent<CanvasScaler>(canvasObj);
        }

        Undo.RecordObject(olcekleyici, "Canvas olcekle");
        olcekleyici.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        olcekleyici.referenceResolution = new Vector2(1920f, 1080f);
        olcekleyici.matchWidthOrHeight = 0.5f;
    }

    private static RectTransform PaneliHazirla(Transform canvas)
    {
        Transform mevcut = canvas.Find("DiyalogPaneli");
        GameObject panel = mevcut != null ? mevcut.gameObject : YeniUiObjesi("DiyalogPaneli", canvas);

        RectTransform rt = panel.GetComponent<RectTransform>();
        Undo.RecordObject(rt, "Panel konum");
        rt.anchorMin = new Vector2(0.5f, 0f);
        rt.anchorMax = new Vector2(0.5f, 0f);
        rt.pivot = new Vector2(0.5f, 0f);
        rt.anchoredPosition = new Vector2(0f, 30f);
        rt.sizeDelta = new Vector2(1100f, 270f);

        Image zemin = panel.GetComponent<Image>();
        if (zemin == null)
        {
            zemin = Undo.AddComponent<Image>(panel);
        }

        Undo.RecordObject(zemin, "Panel renk");
        zemin.color = Zemin;
        zemin.raycastTarget = false;

        // Ust kenarda ince amber cizgi
        Transform cizgiMevcut = panel.transform.Find("UstCizgi");
        GameObject cizgi = cizgiMevcut != null ? cizgiMevcut.gameObject : YeniUiObjesi("UstCizgi", panel.transform);
        RectTransform crt = cizgi.GetComponent<RectTransform>();
        crt.anchorMin = new Vector2(0f, 1f);
        crt.anchorMax = new Vector2(1f, 1f);
        crt.pivot = new Vector2(0.5f, 1f);
        crt.anchoredPosition = Vector2.zero;
        crt.sizeDelta = new Vector2(0f, 3f);
        Image cizgiResmi = cizgi.GetComponent<Image>();
        if (cizgiResmi == null)
        {
            cizgiResmi = cizgi.AddComponent<Image>();
        }

        cizgiResmi.color = Amber;
        cizgiResmi.raycastTarget = false;

        return rt;
    }

    // Bu bir arayuz etiketi, ARGUS'un sozu degil
    private static void BaslikHazirla(RectTransform panel)
    {
        Transform mevcut = panel.Find("ArgusBaslik");
        GameObject baslik = mevcut != null ? mevcut.gameObject : YeniUiObjesi("ArgusBaslik", panel);

        RectTransform rt = baslik.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0f, 1f);
        rt.anchorMax = new Vector2(0f, 1f);
        rt.pivot = new Vector2(0f, 1f);
        rt.anchoredPosition = new Vector2(28f, -14f);
        rt.sizeDelta = new Vector2(600f, 36f);

        TextMeshProUGUI metin = baslik.GetComponent<TextMeshProUGUI>();
        if (metin == null)
        {
            metin = baslik.AddComponent<TextMeshProUGUI>();
        }

        metin.text = "ARGUS  <size=70%><color=#8C8C8C>· Müze Güvenlik Sistemi</color></size>";
        metin.fontSize = 28f;
        metin.fontStyle = FontStyles.Bold;
        metin.color = Amber;
        metin.alignment = TextAlignmentOptions.Left;
        metin.raycastTarget = false;
    }

    private static void CiktiyiYerlestir(GameObject ciktiObj, RectTransform panel)
    {
        Undo.SetTransformParent(ciktiObj.transform, panel, "Ciktiyi panele al");

        RectTransform rt = ciktiObj.GetComponent<RectTransform>();
        Undo.RecordObject(rt, "Cikti konum");
        rt.anchorMin = new Vector2(0f, 0f);
        rt.anchorMax = new Vector2(1f, 1f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.localScale = Vector3.one;
        rt.offsetMin = new Vector2(28f, 84f);
        rt.offsetMax = new Vector2(-28f, -58f);

        TextMeshProUGUI metin = ciktiObj.GetComponent<TextMeshProUGUI>();
        Undo.RecordObject(metin, "Cikti bicim");
        metin.text = "";
        metin.fontSize = 26f;
        metin.fontStyle = FontStyles.Normal;
        metin.color = MetinRengi;
        metin.alignment = TextAlignmentOptions.TopLeft;
        metin.textWrappingMode = TextWrappingModes.Normal;
        metin.overflowMode = TextOverflowModes.Overflow;
        metin.raycastTarget = false;
    }

    private static void GirdiyiYerlestir(GameObject girdiObj, RectTransform panel)
    {
        Undo.SetTransformParent(girdiObj.transform, panel, "Girdiyi panele al");

        RectTransform rt = girdiObj.GetComponent<RectTransform>();
        Undo.RecordObject(rt, "Girdi konum");
        rt.anchorMin = new Vector2(0f, 0f);
        rt.anchorMax = new Vector2(1f, 0f);
        rt.pivot = new Vector2(0.5f, 0f);
        rt.localScale = Vector3.one;
        rt.anchoredPosition = new Vector2(0f, 20f);
        rt.offsetMin = new Vector2(28f, rt.offsetMin.y);
        rt.offsetMax = new Vector2(-28f, rt.offsetMax.y);
        rt.sizeDelta = new Vector2(rt.sizeDelta.x, 50f);

        Image zemin = girdiObj.GetComponent<Image>();
        if (zemin != null)
        {
            Undo.RecordObject(zemin, "Girdi zemin");
            zemin.color = KutuZemini;
        }

        TMP_InputField girdi = girdiObj.GetComponent<TMP_InputField>();
        Undo.RecordObject(girdi, "Girdi ayar");
        girdi.lineType = TMP_InputField.LineType.SingleLine;
        girdi.caretColor = Amber;
        girdi.customCaretColor = true;
        girdi.selectionColor = new Color(Amber.r, Amber.g, Amber.b, 0.35f);

        if (girdi.textComponent != null)
        {
            Undo.RecordObject(girdi.textComponent, "Girdi yazi");
            girdi.textComponent.color = MetinRengi;
            girdi.textComponent.fontSize = 22f;
        }

        TMP_Text ipucu = girdi.placeholder as TMP_Text;
        if (ipucu != null)
        {
            Undo.RecordObject(ipucu, "Girdi ipucu");
            ipucu.text = "ARGUS'a yazmak için Enter, çıkmak için Esc";
            ipucu.color = SoluRenk;
            ipucu.fontStyle = FontStyles.Italic;
            ipucu.fontSize = 22f;
        }
    }

    private static GameObject YeniUiObjesi(string ad, Transform ebeveyn)
    {
        GameObject obj = new GameObject(ad, typeof(RectTransform));
        Undo.RegisterCreatedObjectUndo(obj, "Arayuz objesi ekle");
        obj.layer = LayerMask.NameToLayer("UI");
        obj.transform.SetParent(ebeveyn, false);
        return obj;
    }

    // Panel, ARGUS ile birlikte ikinci panel acilana kadar gizli kalir
    private static void ArgusIleGizle(GameObject panel)
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

        List<Object> mevcut = new List<Object>();
        for (int i = 0; i < liste.arraySize; i++)
        {
            mevcut.Add(liste.GetArrayElementAtIndex(i).objectReferenceValue);
        }

        if (!mevcut.Contains(panel))
        {
            liste.arraySize++;
            liste.GetArrayElementAtIndex(liste.arraySize - 1).objectReferenceValue = panel;
            ayar.ApplyModifiedProperties();
        }
    }
}
