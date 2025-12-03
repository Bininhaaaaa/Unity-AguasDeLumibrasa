using System;
using EasyPeasyFirstPersonController;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class GameplayUIController : MonoBehaviour
{
    public Camera gameplayCamera;
    public GameObject gameplayUI;
    public GameObject SniperAimUI;
    public GameObject CardSelectorUI;
    public GameObject gunModel;


    void Start()
    {
        FirstPersonController.OnPressAim += ShowSniperAim;
        FirstPersonController.OnReleaseAim += HideSniperAim;
        FirstPersonController.OnPressTab += ShowCardSelector;
        FirstPersonController.OnReleaseTab += HideCardSelector;

        CardSelectorUI.SetActive(false);

    }

    void OnDestroy()
    {
        FirstPersonController.OnPressAim -= ShowSniperAim;
        FirstPersonController.OnReleaseAim -= HideSniperAim;
        FirstPersonController.OnPressTab -= ShowCardSelector;
        FirstPersonController.OnReleaseTab -= HideCardSelector;
    }

    private void HideSniperAim()
    {
        gameplayUI.SetActive(true);
        SniperAimUI.SetActive(false);
        gunModel.SetActive(true);
        gameplayCamera.fieldOfView = 60;
    }

    private void ShowSniperAim()
    {
        gameplayCamera.fieldOfView = 20;
        gameplayUI.SetActive(false);
        gunModel.SetActive(false);
        SniperAimUI.SetActive(true);
    }

    private void ShowCardSelector()
    {
        CardSelectorUI.SetActive(true);
        // gameplayUI.SetActive(false);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0.1f;
    }
    private void HideCardSelector()
    {
        CardSelectorUI.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        // gameplayUI.SetActive(true);
        Time.timeScale = 1f;
    }
}
