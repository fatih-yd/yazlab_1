using System;

// ARGUS'un JSON cevabinin karsiligi. Alan adlari sozlesmedekiyle ayni tutuldu.
[Serializable]
public class ArgusCevap
{
    public string reply;
    public bool unlock_vault;

    // Hata durumunda kullanilan guvenli cevap. Kasa kapali kalir.
    public static ArgusCevap Yedek()
    {
        return new ArgusCevap
        {
            reply = "Şu an konuşamıyorum.",
            unlock_vault = false
        };
    }
}
