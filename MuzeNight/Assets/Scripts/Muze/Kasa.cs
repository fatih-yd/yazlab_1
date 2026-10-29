using System;
using UnityEngine;

// Kilitli kasa. Acilmasi icin hem elektrigin acik olmasi hem de ARGUS'un izin vermesi gerekir.
public class Kasa : MonoBehaviour
{
    [SerializeField] private PowerPanel elektrikPaneli;
    [SerializeField] private Renderer gosterge;

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
        KasaAcildi?.Invoke();
        return true;
    }
}
