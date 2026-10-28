using UnityEngine;

// Tozcu panelin basinda. Elektrigi acar, gorevi biter.
public class PanelindeDurumu : ITozcuDurumu
{
    public void Enter(TozcuKontrolcu tozcu)
    {
        tozcu.Agent.ResetPath();
        tozcu.HedefPanel.ElektrigiAc();
        Debug.Log("Tozcu: panele ulasti.");
    }

    public void Update(TozcuKontrolcu tozcu)
    {
    }

    public void Exit(TozcuKontrolcu tozcu)
    {
    }
}
