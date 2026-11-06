using UnityEngine;
using UnityEngine.InputSystem;

// Birinci sahis oyuncu. Fareyle serbestce bakar, WASD ile yurur, G ile Tozcu'ya komut verir.
[RequireComponent(typeof(CharacterController))]
public class OyuncuKontrolcu : MonoBehaviour
{
    // Terminal yazi yazarken true olur. Boylece harfler hareket ya da komut olarak algilanmaz.
    public static bool HareketKilitli;

    // Oyun bittiginde true olur. Hareket ve komutlar tamamen durur.
    public static bool OyunBitti;

    // Esc menusu acikken true olur
    public static bool Duraklatildi;

    [SerializeField] private float hiz = 4f;
    [SerializeField] private float kosmaCarpani = 1.7f;
    [SerializeField] private Transform kamera;
    [SerializeField] private TozcuKontrolcu tozcu;

    // Birinci sahis kamerada kendi govdemizi gormeyelim
    [SerializeField] private bool govdeyiGizle = true;

    private CharacterController karakter;
    private float dikeyHiz;
    private float bakisEgimi;

    private void Awake()
    {
        karakter = GetComponent<CharacterController>();

        // Bu degiskenler static, ana menuden tekrar baslayinca eski oyundan kalmasin
        HareketKilitli = false;
        OyunBitti = false;
        Duraklatildi = false;

        if (govdeyiGizle)
        {
            Renderer govde = GetComponent<Renderer>();
            if (govde != null)
            {
                govde.enabled = false;
            }
        }
    }

    private void Update()
    {
        Keyboard klavye = Keyboard.current;
        Mouse fare = Mouse.current;
        if (klavye == null || fare == null)
        {
            return;
        }

        bool oynuyor = !HareketKilitli && !OyunBitti && !Duraklatildi;
        ImleciAyarla(oynuyor);

        Vector3 yon = Vector3.zero;

        if (oynuyor)
        {
            BakisiGuncelle(fare);
            yon = HareketYonu(klavye);

            if (klavye.gKey.wasPressedThisFrame && tozcu != null)
            {
                tozcu.PaneleGitKomutu();
            }
        }

        // Yer cekimi: karakter yere basarken dikeyHiz sabit kalsin
        if (karakter.isGrounded && dikeyHiz < 0f)
        {
            dikeyHiz = -2f;
        }
        dikeyHiz += Physics.gravity.y * Time.deltaTime;

        // Sol Shift basiliyken kosar
        float anlikHiz = klavye.leftShiftKey.isPressed ? hiz * kosmaCarpani : hiz;
        Vector3 hareket = yon * anlikHiz + Vector3.up * dikeyHiz;
        karakter.Move(hareket * Time.deltaTime);
    }

    // Oynarken imlec ortada kilitli ve gizli, terminal acikken ya da oyun bitince serbest
    private void ImleciAyarla(bool oynuyor)
    {
        CursorLockMode istenen = oynuyor ? CursorLockMode.Locked : CursorLockMode.None;
        if (Cursor.lockState != istenen)
        {
            Cursor.lockState = istenen;
            Cursor.visible = !oynuyor;
        }
    }

    // Fare hareketi bakisi dondurur: yatayda govde, dikeyde sadece kamera
    private void BakisiGuncelle(Mouse fare)
    {
        Vector2 delta = fare.delta.ReadValue();

        // Hassasiyet ayarlar menusunden gelir
        float fareHassasiyeti = Ayarlar.FareHassasiyeti;

        transform.Rotate(0f, delta.x * fareHassasiyeti, 0f);

        bakisEgimi = Mathf.Clamp(bakisEgimi - delta.y * fareHassasiyeti, -80f, 80f);
        if (kamera != null)
        {
            kamera.localRotation = Quaternion.Euler(bakisEgimi, 0f, 0f);
        }
    }

    // Bakilan yone gore hareket: W = ileri, D = sag (karakterin kendi eksenine gore)
    private Vector3 HareketYonu(Keyboard klavye)
    {
        float yatay = 0f;
        float dikey = 0f;

        if (klavye.aKey.isPressed) yatay -= 1f;
        if (klavye.dKey.isPressed) yatay += 1f;
        if (klavye.sKey.isPressed) dikey -= 1f;
        if (klavye.wKey.isPressed) dikey += 1f;

        Vector3 yon = transform.right * yatay + transform.forward * dikey;
        return yon.normalized;
    }
}
