using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIProgressiveUlt : MonoBehaviour
{
    public float fillTime = 1f;
    public Image fillImage;
    public TextMeshProUGUI progressText;
    private float timer = 0f;
    private int steps = 16;

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime / fillTime;
        if (timer >= 1f)
        {
            timer = 0f;
        }
        int step = Mathf.RoundToInt(timer * steps);
        fillImage.fillAmount = step / (float)steps;
        progressText.SetText(Mathf.RoundToInt(timer * 100f) + "%");
    }
}

