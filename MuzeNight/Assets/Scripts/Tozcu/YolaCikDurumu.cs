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
        // Once hedefe vardi mi kontrol et. Hedef panelin kendisi raycast'te engel gibi gorunebilir,
        // bu yuzden varis kontrolu engel kontrolunden once yapilir.
        if (tozcu.PaneleVardiMi())
        {
            tozcu.DurumDegistir(new PanelindeDurumu());
            return;
        }

        // Hedefe cok yakinken isin, hedefin kendisini engel sanabilir.
        // Son yaklasmada NavMesh zaten engelleri asar, bu yuzden isini kontrolunu atliyoruz.
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
