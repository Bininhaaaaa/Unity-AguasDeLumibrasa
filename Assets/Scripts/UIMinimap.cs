using UnityEngine;
using UnityEngine.UI;

public class UIMinimap : MonoBehaviour
{

    public Transform minimapTransform;
    public Transform playerTransform;

    private Image minimapImage;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        minimapImage = minimapTransform.GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        Transform rotationSource = playerTransform;
        minimapImage.rectTransform.localEulerAngles = new Vector3(0, 0, rotationSource.eulerAngles.y);
    }
}

