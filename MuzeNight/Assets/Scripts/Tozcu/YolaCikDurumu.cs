// Panelin erisim noktasina dogru yuruyor.
// Yolda engel gorurse EngelAlgilandi durumuna gecer.
// Panele varirsa Panelinde durumuna gecer.
public class YolaCikDurumu : ITozcuDurumu
{
    public void Enter(TozcuKontrolcu tozcu)
    {
        tozcu.Agent.SetDestination(tozcu.HedefPanel.ErisimNoktasi.position);
    }

    public void Update(TozcuKontrolcu tozcu)
    {
        if (tozcu.Sensor.EngelVarMi())
        {
            tozcu.DurumDegistir(new EngelAlgilandiDurumu());
            return;
        }

        if (tozcu.PaneleVardiMi())
        {
            tozcu.DurumDegistir(new PanelindeDurumu());
        }
    }

    public void Exit(TozcuKontrolcu tozcu)
    {
    }
}
