
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardSelectable : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    [SerializeField] Image cardBackgroundImage;
    [SerializeField] private Sprite normalBackground;
    [SerializeField] private Sprite hoveredBackground;
    [SerializeField] private Transform cardIconTransform;

    [SerializeField] private GameObject cardDescriptionPanel;

    [SerializeField] private float cardSizeMultiplauer = 1.1f;
    private Vector3 originalScale;

    private void Reset()
    {
        cardBackgroundImage.sprite = normalBackground;
        cardIconTransform.localScale = originalScale;
        cardDescriptionPanel.SetActive(false);
    }

    void OnDisable()
    {
        Reset();
    }

    private void Start()
    {
        originalScale = cardIconTransform.localScale;
        cardDescriptionPanel.SetActive(false);
        cardBackgroundImage.sprite = normalBackground;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        cardIconTransform.localScale = originalScale * cardSizeMultiplauer;
        cardDescriptionPanel.SetActive(true);
        cardBackgroundImage.sprite = hoveredBackground;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        cardIconTransform.localScale = originalScale;
        cardDescriptionPanel.SetActive(false);
        cardBackgroundImage.sprite = normalBackground;
    }


}
