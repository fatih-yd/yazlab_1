using UnityEngine;

// Sigorta paneli. Tozcu erisim noktasina gelip ElektrigiAc'i cagirinca elektrik acilir.
public class PowerPanel : MonoBehaviour
{
    [SerializeField] private Transform erisimNoktasi;
    [SerializeField] private Renderer gosterge;

    public bool ElektrikAcikMi { get; private set; }
    public Transform ErisimNoktasi => erisimNoktasi;

    public void ElektrigiAc()
    {
        if (ElektrikAcikMi)
        {
            return;
        }

        ElektrikAcikMi = true;

        if (gosterge != null)
        {
            gosterge.material.color = Color.green;
        }

        Debug.Log("Elektrik acildi.");
    }
}
