using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DEMOcontroller : MonoBehaviour
{
    [SerializeField] private GameObject redirectConfirmation;
    [SerializeField] private GameObject qrCodeImage;
    [SerializeField] private string steamlink = "steamstore.com/example";

    private void Awake()
    {
        redirectConfirmation.SetActive(false);
        if (qrCodeImage) qrCodeImage.SetActive(false);
    }

    public void OpenQRCode()
    {
        if (FixScrollRect.isDragging) return;
        qrCodeImage.SetActive(true);
    }

    public void OpenRedirectToSteamConfirmation()
    {
        if (FixScrollRect.isDragging) return;
        redirectConfirmation.SetActive(true);
    }

    public void CloseRedirectToSteamConfirmation()
    {
        redirectConfirmation.SetActive(false);
    }

    public void RedirectToSteam()
    {
        Application.OpenURL(steamlink);
        CloseRedirectToSteamConfirmation();
    }
}
