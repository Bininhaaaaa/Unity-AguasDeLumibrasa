using System;
using EasyPeasyFirstPersonController;
using UnityEngine;

public class GameplayUIController : MonoBehaviour
{
    public Camera gameplayCamera;
    public GameObject gameplayUI;
    public GameObject SniperAimUI;


    void Start()
    {
        FirstPersonController.OnPressAim += ShowSniperAim;
        FirstPersonController.OnReleaseAim += HideSniperAim;
    }

    void OnDestroy()
    {
        FirstPersonController.OnPressAim -= ShowSniperAim;
        FirstPersonController.OnReleaseAim -= HideSniperAim;
    }

    private void HideSniperAim()
    {
        gameplayUI.SetActive(true);
        SniperAimUI.SetActive(false);
        gameplayCamera.fieldOfView = 60;
    }

    private void ShowSniperAim()
    {
        gameplayCamera.fieldOfView = 20;
        gameplayUI.SetActive(false);
        SniperAimUI.SetActive(true);
    }
}
