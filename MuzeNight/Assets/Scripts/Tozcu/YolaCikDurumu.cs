// Panelin erisim noktasina dogru yuruyor.
public class YolaCikDurumu : ITozcuDurumu
{
    public void Enter(TozcuKontrolcu tozcu)
    {
        tozcu.Agent.SetDestination(tozcu.HedefPanel.ErisimNoktasi.position);
    }

    public void Update(TozcuKontrolcu tozcu)
    {
        // Once hedefe vardi mi kontrol et.
        if (tozcu.PaneleVardiMi())
        {
            tozcu.DurumDegistir(new PanelindeDurumu());
            return;
        }

        // Hedefe cok yakinken isin, hedefin kendisini engel sanabilir.
        bool hedefeYakin = tozcu.HedefeUzaklik < 2f;

        if (!hedefeYakin && tozcu.EngelSogumaBitti && tozcu.Sensor.EngelVarMi())
        {
            tozcu.EngelIsaretle();
            tozcu.DurumDegistir(new EngelAlgilandiDurumu());
        }
    }

    public void Exit(TozcuKontrolcu tozcu)
    {
    }
}
