using UnityEngine;
using UnityEngine.InputSystem;

public class UIPointerFollowMouse : MonoBehaviour
{
    public RectTransform rectTransform;
    Canvas canvas;

    void Awake()
    {
        //rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
    }

    void Update()
    {
        if (Mouse.current == null) return;

        Vector2 mousePos = Mouse.current.position.ReadValue();
        Vector2 localPoint;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            mousePos,
            canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera,
            out localPoint
        );

        Vector2 direction = localPoint - rectTransform.anchoredPosition;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        rectTransform.rotation = Quaternion.Euler(0f, 0f, angle - 90f);
    }
}
