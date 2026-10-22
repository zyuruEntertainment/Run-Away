
using UnityEngine;
using UnityEngine.EventSystems;

public class MobileJoystick : MonoBehaviour,
    IPointerDownHandler,
    IDragHandler,
    IPointerUpHandler
{
    [Header("Joystick")]
    public RectTransform handle;

    public Vector2 InputDirection { get; private set; }

    private RectTransform joystickRect;
    private Canvas canvas;

    private void Awake()
    {
        joystickRect = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        OnDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (joystickRect == null || handle == null)
            return;

        Camera uiCamera = null;

        if (canvas != null &&
            canvas.renderMode != RenderMode.ScreenSpaceOverlay)
        {
            uiCamera = canvas.worldCamera;
        }

        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
                joystickRect,
                eventData.position,
                uiCamera,
                out Vector2 localPoint))
        {
            return;
        }

        float radius = Mathf.Min(
            joystickRect.rect.width,
            joystickRect.rect.height
        ) * 0.5f;

        if (radius <= 0f)
            return;

        Vector2 direction = localPoint / radius;
        InputDirection = Vector2.ClampMagnitude(direction, 1f);

        handle.anchoredPosition = InputDirection * radius;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        InputDirection = Vector2.zero;

        if (handle != null)
            handle.anchoredPosition = Vector2.zero;
    }
}
