using UnityEngine;

// Oyuncu ayarlari. PlayerPrefs'e kaydedilir, oyun kapanip acilinca da kalir.
public static class Ayarlar
{
    private const string FareAnahtari = "FareHassasiyeti";
    private const string SesAnahtari = "SesSeviyesi";
    private const string TamEkranAnahtari = "TamEkran";

    private static bool yuklendi;
    private static float fare;
    private static float ses;
    private static bool tamEkran;

    public static float FareHassasiyeti
    {
        get { Yukle(); return fare; }
        set { Yukle(); fare = value; PlayerPrefs.SetFloat(FareAnahtari, value); }
    }

    public static float Ses
    {
        get { Yukle(); return ses; }
        set { Yukle(); ses = value; PlayerPrefs.SetFloat(SesAnahtari, value); AudioListener.volume = value; }
    }

    public static bool TamEkran
    {
        get { Yukle(); return tamEkran; }
        set { Yukle(); tamEkran = value; PlayerPrefs.SetInt(TamEkranAnahtari, value ? 1 : 0); Screen.fullScreen = value; }
    }

    // Kayitli ayarlari oyuna uygular (oyun ve menu acilirken cagrilir)
    public static void Uygula()
    {
        Yukle();
        AudioListener.volume = ses;
        Screen.fullScreen = tamEkran;
    }

    private static void Yukle()
    {
        if (yuklendi)
        {
            return;
        }

        fare = PlayerPrefs.GetFloat(FareAnahtari, 0.1f);
        ses = PlayerPrefs.GetFloat(SesAnahtari, 1f);
        tamEkran = PlayerPrefs.GetInt(TamEkranAnahtari, 1) == 1;
        yuklendi = true;
    }
}
