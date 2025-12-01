using EasyPeasyFirstPersonController;
using UnityEngine;
using UnityEngine.UI;

public class UIStamina : MonoBehaviour
{
    private Image staminaFillImage;
    public FirstPersonController playerController;

    void Start()
    {
        staminaFillImage = GetComponent<Image>();
        // playerController = FindFirstObjectByType<FirstPersonController>();
    }

    // Update is called once per frame
    void Update()
    {
        float fillAmount = playerController.currentStamina / playerController.maxStamina;
        staminaFillImage.fillAmount = fillAmount;
    }
}

