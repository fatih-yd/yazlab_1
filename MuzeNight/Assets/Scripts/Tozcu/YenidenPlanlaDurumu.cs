using UnityEngine;

// Kisa bir bekleme sonrasi YolaCik durumuna donerek yolu yeniden hesaplatir.
public class YenidenPlanlaDurumu : ITozcuDurumu
{
    private float gecenSure;

    public void Enter(TozcuKontrolcu tozcu)
    {
        gecenSure = 0f;
    }

    public void Update(TozcuKontrolcu tozcu)
    {
        gecenSure += Time.deltaTime;
        if (gecenSure < tozcu.YenidenPlanlamaBeklemesi)
        {
            return;
        }

        // YolaCik'in Enter'i hedefi yeniden verir, ajan yolu sifirdan hesaplar.
        tozcu.DurumDegistir(new YolaCikDurumu());
    }

    public void Exit(TozcuKontrolcu tozcu)
    {
    }
}
