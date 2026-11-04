using System;
using System.Collections;
using UnityEngine;

// Kilitli kasa. Acilmasi icin hem elektrigin acik olmasi hem de ARGUS'un izin vermesi gerekir.
public class Kasa : MonoBehaviour
{
    [SerializeField] private PowerPanel elektrikPaneli;
    [SerializeField] private Renderer gosterge;
    [SerializeField] private Transform kapiMenteseli;
    [SerializeField] private float acilmaAcisi = 100f;
    [SerializeField] private float acilmaSuresi = 1.5f;

    public bool AcikMi { get; private set; }

    // Kasa acildiginda tetiklenir (oyun sonu ekrani bunu dinler)
    public event Action KasaAcildi;

    // ARGUS'un kararini uygular. Elektrik kapaliysa izin verilse bile kasa acilmaz.
    public bool ArgusKarariniUygula(bool izinVerdi)
    {
        if (AcikMi)
        {
            return true;
        }

        if (!izinVerdi)
        {
            return false;
        }

        if (elektrikPaneli == null || !elektrikPaneli.ElektrikAcikMi)
        {
            Debug.Log("Kasa: elektrik kapali, izin verilmesine ragmen acilmadi.");
            return false;
        }

        AcikMi = true;

        if (gosterge != null)
        {
            gosterge.material.color = Color.cyan;
        }

        Debug.Log("Kasa acildi.");

        if (kapiMenteseli != null)
        {
            StartCoroutine(KapiyiAc());
        }

        KasaAcildi?.Invoke();
        return true;
    }

    // Kapi, menteşe noktasi etrafinda yumusakca doner
    private IEnumerator KapiyiAc()
    {
        Quaternion baslangic = kapiMenteseli.localRotation;
        Quaternion hedef = baslangic * Quaternion.Euler(0f, acilmaAcisi, 0f);
        float gecen = 0f;

        while (gecen < acilmaSuresi)
        {
            gecen += Time.deltaTime;
            kapiMenteseli.localRotation = Quaternion.Slerp(baslangic, hedef, gecen / acilmaSuresi);
            yield return null;
        }

        kapiMenteseli.localRotation = hedef;
    }
}
