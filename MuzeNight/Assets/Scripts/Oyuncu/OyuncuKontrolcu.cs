using UnityEngine;
using UnityEngine.InputSystem;

// Oyuncu karakteri. WASD ile yurur, G tusu ile Tozcu'ya "sigorta paneline git" komutu verir.
[RequireComponent(typeof(CharacterController))]
public class OyuncuKontrolcu : MonoBehaviour
{
    // Terminal yazi yazarken true olur. Boylece harfler hareket ya da komut olarak algilanmaz.
    public static bool HareketKilitli;

    [SerializeField] private float hiz = 4f;
    [SerializeField] private TozcuKontrolcu tozcu;

    private CharacterController karakter;
    private float dikeyHiz;

    private void Awake()
    {
        karakter = GetComponent<CharacterController>();
    }

    private void Update()
    {
        Keyboard klavye = Keyboard.current;
        if (klavye == null)
        {
            return;
        }

        Vector3 yon = Vector3.zero;
        if (!HareketKilitli)
        {
            float yatay = 0f;
            float dikey = 0f;
            if (klavye.aKey.isPressed) yatay -= 1f;
            if (klavye.dKey.isPressed) yatay += 1f;
            if (klavye.sKey.isPressed) dikey -= 1f;
            if (klavye.wKey.isPressed) dikey += 1f;

            // Dunya eksenine gore yuruyoruz: W = ileri (+Z), D = sag (+X)
            yon = new Vector3(yatay, 0f, dikey).normalized;

            if (klavye.gKey.wasPressedThisFrame)
            {
                if (tozcu == null)
                {
                    Debug.LogWarning("Oyuncu: Tozcu alani bos. Inspector'da Tozcu'yu ata.");
                }
                else
                {
                    tozcu.PaneleGitKomutu();
                }
            }
        }

        // Yer cekimi: karakter yere basarken dikeyHiz sabit kalsin
        if (karakter.isGrounded && dikeyHiz < 0f)
        {
            dikeyHiz = -2f;
        }
        dikeyHiz += Physics.gravity.y * Time.deltaTime;

        Vector3 hareket = yon * hiz + Vector3.up * dikeyHiz;
        karakter.Move(hareket * Time.deltaTime);
    }
}
