using System.Collections;
using UnityEngine;
using UnityEngine.AI;

// Belirli araliklarla yerden yukselip alcalan bariyer.
// Kapaliyken NavMesh'te engel olur (carve), acikken engel kalkar.
// Tozcu yolunu bariyerin o anki durumuna gore kendisi yeniden bulur.
public class HareketliBariyer : MonoBehaviour
{
    [SerializeField] private bool kapaliBasla = true;
    [SerializeField] private float beklemeSuresi = 4f;
    [SerializeField] private float hareketSuresi = 1f;
    [SerializeField] private float inisMesafesi = 3.1f;

    private NavMeshObstacle navEngeli;
    private Collider carpisma;
    private Vector3 kapaliKonum;
    private Vector3 acikKonum;

    private void Awake()
    {
        navEngeli = GetComponent<NavMeshObstacle>();
        carpisma = GetComponent<Collider>();

        // Bariyer sahnede kapali (yukarida) konumda durur, acikken zeminin altina iner
        kapaliKonum = transform.position;
        acikKonum = kapaliKonum + Vector3.down * inisMesafesi;
    }

    private void Start()
    {
        transform.position = kapaliBasla ? kapaliKonum : acikKonum;
        EngelOlsun(kapaliBasla);
        StartCoroutine(Dongu());
    }

    private IEnumerator Dongu()
    {
        bool kapali = kapaliBasla;

        while (true)
        {
            yield return new WaitForSeconds(beklemeSuresi);

            if (kapali)
            {
                // Inmeye baslar baslamaz yol acik sayilir
                EngelOlsun(false);
                yield return Hareket(kapaliKonum, acikKonum);
            }
            else
            {
                // Yukselmeye baslar baslamaz yol kapali sayilir, Tozcu erken yeniden planlar
                EngelOlsun(true);
                yield return Hareket(acikKonum, kapaliKonum);
            }

            kapali = !kapali;
        }
    }

    private IEnumerator Hareket(Vector3 baslangic, Vector3 hedef)
    {
        float gecen = 0f;

        while (gecen < hareketSuresi)
        {
            gecen += Time.deltaTime;
            transform.position = Vector3.Lerp(baslangic, hedef, gecen / hareketSuresi);
            yield return null;
        }

        transform.position = hedef;
    }

    private void EngelOlsun(bool durum)
    {
        if (navEngeli != null)
        {
            navEngeli.enabled = durum;
        }

        if (carpisma != null)
        {
            carpisma.enabled = durum;
        }
    }
}
