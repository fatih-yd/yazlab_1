// Tozcu'nun her durumunun uymasi gereken ortak sozlesme.
public interface ITozcuDurumu
{
    // Duruma ilk girildiginde bir kez calisir.
    void Enter(TozcuKontrolcu tozcu);

    // Her karede calisir. Durum degisikligi burada yapilir.
    void Update(TozcuKontrolcu tozcu);

    // Durumdan cikarken bir kez calisir.
    void Exit(TozcuKontrolcu tozcu);
}
