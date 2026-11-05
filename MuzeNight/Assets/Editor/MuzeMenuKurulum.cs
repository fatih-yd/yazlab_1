using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;

// Tools → Muze → Ana Menu Sahnesini Kur menusu.
// AnaMenu sahnesini (kamera, EventSystem, AnaMenu bileseni) olusturur ve
// build listesine once AnaMenu, sonra muze sahnesini ekler.
public static class MuzeMenuKurulum
{
    private const string MenuYolu = "Assets/Scenes/AnaMenu.unity";
    private const string OyunYolu = "Assets/Scenes/muze.unity";

    [MenuItem("Tools/Muze/Ana Menu Sahnesini Kur")]
    public static void AnaMenuSahnesiniKur()
    {
        // Acik sahnede kaydedilmemis degisiklik varsa once sor
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            return;
        }

        var sahne = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        GameObject kameraNesnesi = new GameObject("Main Camera", typeof(Camera), typeof(AudioListener));
        kameraNesnesi.tag = "MainCamera";
        Camera kamera = kameraNesnesi.GetComponent<Camera>();
        kamera.clearFlags = CameraClearFlags.SolidColor;
        kamera.backgroundColor = new Color(0.02f, 0.02f, 0.04f);

        new GameObject("EventSystem", typeof(EventSystem), typeof(InputSystemUIInputModule));
        new GameObject("AnaMenu").AddComponent<AnaMenu>();

        EditorSceneManager.SaveScene(sahne, MenuYolu);

        EditorBuildSettings.scenes = new EditorBuildSettingsScene[]
        {
            new EditorBuildSettingsScene(MenuYolu, true),
            new EditorBuildSettingsScene(OyunYolu, true)
        };

        Debug.Log("Ana menu sahnesi kuruldu ve build listesine eklendi (AnaMenu, muze).");
    }
}
