
using UnityEngine;
using UnityEngine.EventSystems;

public class MobileLookArea : MonoBehaviour,
    IPointerDownHandler,
    IDragHandler,
    IPointerUpHandler
{
    private Vector2 lastPosition;
    private Vector2 lookDelta;
    private int activePointerId = int.MinValue;

    public void OnPointerDown(PointerEventData eventData)
    {
        // Hanya satu jari yang mengendalikan kamera.
        if (activePointerId != int.MinValue)
            return;

        activePointerId = eventData.pointerId;
        lastPosition = eventData.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (eventData.pointerId != activePointerId)
            return;

        Vector2 currentPosition = eventData.position;

        lookDelta += currentPosition - lastPosition;
        lastPosition = currentPosition;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.pointerId != activePointerId)
            return;

        activePointerId = int.MinValue;
    }

    public Vector2 ConsumeDelta()
    {
        Vector2 result = lookDelta;
        lookDelta = Vector2.zero;
        return result;
    }

    private void OnDisable()
    {
        activePointerId = int.MinValue;
        lookDelta = Vector2.zero;
    }
}
