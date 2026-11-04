using System;
using System.Collections;
using UnityEngine;

// Sigorta paneli. Tozcu erisim noktasina gelip ElektrigiAc'i cagirinca elektrik acilir.
public class PowerPanel : MonoBehaviour
{
    [SerializeField] private Transform erisimNoktasi;
    [SerializeField] private Renderer gosterge;
    [SerializeField] private Color kapaliRenk = Color.red;
    [SerializeField] private Color acikRenk = Color.green;
    [SerializeField] private float gecisSuresi = 1f;

    public bool ElektrikAcikMi { get; private set; }
    public Transform ErisimNoktasi => erisimNoktasi;

    // Elektrik acilinca tetiklenir (gecit kapisi gibi sistemler dinler)
    public event Action ElektrikAcildi;

    private void Start()
    {
        if (gosterge != null)
        {
            gosterge.material.color = kapaliRenk;
        }
    }

    public void ElektrigiAc()
    {
        if (ElektrikAcikMi)
        {
            return;
        }

        ElektrikAcikMi = true;

        if (gosterge != null)
        {
            StartCoroutine(RenkGecisi());
        }

        Debug.Log("Elektrik acildi.");
        ElektrikAcildi?.Invoke();
    }

    // Renk kirmizidan yesile yumusak bir gecisle degisir
    private IEnumerator RenkGecisi()
    {
        Material malzeme = gosterge.material;
        float gecen = 0f;

        while (gecen < gecisSuresi)
        {
            gecen += Time.deltaTime;
            malzeme.color = Color.Lerp(kapaliRenk, acikRenk, gecen / gecisSuresi);
            yield return null;
        }

        malzeme.color = acikRenk;
    }
}
