using System.Collections;
using UnityEngine;
using UnityEngine.AI;

// Elektrik acilinca yukari kayarak acilan gecit kapisi.
public class GecitKapisi : MonoBehaviour
{
    [SerializeField] private PowerPanel tetikleyiciPanel;
    [SerializeField] private float acilmaYuksekligi = 3f;
    [SerializeField] private float acilmaSuresi = 1.5f;
    [SerializeField] private NavMeshObstacle navEngeli;
    [SerializeField] private Collider carpisma;

    private void OnEnable()
    {
        if (tetikleyiciPanel != null)
        {
            tetikleyiciPanel.ElektrikAcildi += Ac;
        }
    }

    private void OnDisable()
    {
        if (tetikleyiciPanel != null)
        {
            tetikleyiciPanel.ElektrikAcildi -= Ac;
        }
    }

    private void Ac()
    {
        StartCoroutine(YukariKaldir());
    }

    private IEnumerator YukariKaldir()
    {
        if (navEngeli != null)
        {
            navEngeli.enabled = false;
        }

        if (carpisma != null)
        {
            carpisma.enabled = false;
        }

        Vector3 baslangic = transform.position;
        Vector3 hedef = baslangic + Vector3.up * acilmaYuksekligi;
        float gecen = 0f;

        while (gecen < acilmaSuresi)
        {
            gecen += Time.deltaTime;
            transform.position = Vector3.Lerp(baslangic, hedef, gecen / acilmaSuresi);
            yield return null;
        }

        transform.position = hedef;
        Debug.Log("Gecit kapisi acildi.");
    }
}
