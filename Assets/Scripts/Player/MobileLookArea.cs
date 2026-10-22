
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
    private bool trackingButtonPointer;

    private void OnEnable()
    {
        ResetTracking();
    }

    // Sentuhan yang dimulai di area kamera.
    public void OnPointerDown(PointerEventData eventData)
    {
        if (activePointerId != int.MinValue)
            return;

        activePointerId = eventData.pointerId;
        lastPosition = eventData.position;
        trackingButtonPointer = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (trackingButtonPointer ||
            eventData.pointerId != activePointerId)
            return;

        AddLookMovement(eventData.position);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (trackingButtonPointer ||
            eventData.pointerId != activePointerId)
            return;

        ResetTracking();
    }

    // Dipanggil saat jari mulai menekan Run atau Jump.
    public void BeginLookFromButton(BaseEventData eventData)
    {
        if (!(eventData is PointerEventData pointerData))
            return;

        if (activePointerId != int.MinValue)
            return;

        activePointerId = pointerData.pointerId;
        lastPosition = pointerData.position;
        trackingButtonPointer = true;
    }

    // Dipanggil setiap jari bergerak ketika menekan Run atau Jump.
    public void DragLookFromButton(BaseEventData eventData)
    {
        if (!(eventData is PointerEventData pointerData))
            return;

        if (!trackingButtonPointer ||
            pointerData.pointerId != activePointerId)
            return;

        AddLookMovement(pointerData.position);
    }

    // Dipanggil saat jari dilepas dari Run atau Jump.
    public void EndLookFromButton(BaseEventData eventData)
    {
        if (!(eventData is PointerEventData pointerData))
            return;

        if (!trackingButtonPointer ||
            pointerData.pointerId != activePointerId)
            return;

        ResetTracking();
    }

    private void AddLookMovement(Vector2 currentPosition)
    {
        lookDelta += currentPosition - lastPosition;
        lastPosition = currentPosition;
    }

    public Vector2 ConsumeDelta()
    {
        Vector2 result = lookDelta;
        lookDelta = Vector2.zero;
        return result;
    }

    private void ResetTracking()
    {
        activePointerId = int.MinValue;
        trackingButtonPointer = false;
        lookDelta = Vector2.zero;
    }

    private void OnDisable()
    {
        ResetTracking();
    }
}
