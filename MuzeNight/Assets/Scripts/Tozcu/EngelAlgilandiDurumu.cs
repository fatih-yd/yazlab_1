using UnityEngine;

// Onunde engel gorduk. Hareketi durdurur, bir sonraki karede YenidenPlanla'ya gecer.
public class EngelAlgilandiDurumu : ITozcuDurumu
{
    public void Enter(TozcuKontrolcu tozcu)
    {
        tozcu.Agent.ResetPath();
        Debug.Log("Tozcu: engel algilandi.");
    }

    public void Update(TozcuKontrolcu tozcu)
    {
        tozcu.DurumDegistir(new YenidenPlanlaDurumu());
    }

    public void Exit(TozcuKontrolcu tozcu)
    {
    }
}
