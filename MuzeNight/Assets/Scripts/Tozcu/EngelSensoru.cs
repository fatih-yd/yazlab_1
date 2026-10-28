using UnityEngine;

// Tozcu'nun onune isin atip engel olup olmadigini soyler.
public class EngelSensoru
{
    private readonly Transform kaynak;
    private readonly float mesafe;

    public EngelSensoru(Transform kaynak, float mesafe)
    {
        this.kaynak = kaynak;
        this.mesafe = mesafe;
    }

    public bool EngelVarMi()
    {
        // Isini govdenin biraz onundan atiyoruz, boylece kendi kapsul colliderimizi vurmayiz.
        Vector3 baslangic = kaynak.position + Vector3.up * 0.5f + kaynak.forward * 0.6f;
        bool vurdu = Physics.Raycast(baslangic, kaynak.forward, mesafe);

        // Sahne penceresinde isini gormek icin (kirmizi = engel var).
        Debug.DrawRay(baslangic, kaynak.forward * mesafe, vurdu ? Color.red : Color.green);
        return vurdu;
    }
}
