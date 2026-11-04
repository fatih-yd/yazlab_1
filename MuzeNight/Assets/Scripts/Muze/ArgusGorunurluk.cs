using System;
using UnityEngine;

// ARGUS terminali ve arayuzu, tetikleyici panelin elektrigi acilana kadar gizli kalir.
// Tozcu ikinci paneli acinca ARGUS belirir ve oyuncu onunla konusabilir.
public class ArgusGorunurluk : MonoBehaviour
{
    [SerializeField] private PowerPanel tetikleyiciPanel;
    [SerializeField] private GameObject[] gizlenecekler;

    // ARGUS gorundugunde tetiklenir (terminal karsilama konusmasini baslatir)
    public event Action Belirdi;

    private void Awake()
    {
        Goster(false);
    }

    private void OnEnable()
    {
        if (tetikleyiciPanel != null)
        {
            tetikleyiciPanel.ElektrikAcildi += ElektrikGeldi;
        }
    }

    private void OnDisable()
    {
        if (tetikleyiciPanel != null)
        {
            tetikleyiciPanel.ElektrikAcildi -= ElektrikGeldi;
        }
    }

    private void ElektrikGeldi()
    {
        Goster(true);
        Debug.Log("ARGUS cevrimici.");
        Belirdi?.Invoke();
    }

    private void Goster(bool gorunur)
    {
        if (gizlenecekler == null)
        {
            return;
        }

        foreach (GameObject obj in gizlenecekler)
        {
            if (obj != null)
            {
                obj.SetActive(gorunur);
            }
        }
    }
}
