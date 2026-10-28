// Tozcu komut gelene kadar bekler, hareket etmez.
public class BekleDurumu : ITozcuDurumu
{
    public void Enter(TozcuKontrolcu tozcu)
    {
        tozcu.Agent.ResetPath();
    }

    public void Update(TozcuKontrolcu tozcu)
    {
    }

    public void Exit(TozcuKontrolcu tozcu)
    {
    }
}
