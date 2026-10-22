
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class MobileLookArea : MonoBehaviour,
    IPointerDownHandler,
    IDragHandler,
    IPointerUpHandler
{
    private Vector2 lastPosition;
    private Vector2 lookDelta;
    private int activePointerId = int.MinValue;

    // True jika sentuhan dimulai dari tombol Run atau Jump.
    private bool trackingButtonPointer;

    private void OnEnable()
    {
        trackingButtonPointer = false;
        activePointerId = int.MinValue;
        lookDelta = Vector2.zero;
    }

    // Input normal ketika jari dimulai di area kamera.
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
        if (trackingButtonPointer)
            return;

        if (eventData.pointerId != activePointerId)
            return;

        Vector2 currentPosition = eventData.position;

        lookDelta += currentPosition - lastPosition;
        lastPosition = currentPosition;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.pointerId != activePointerId ||
            trackingButtonPointer)
            return;

        ResetTracking();
    }

    // Hubungkan ke Pointer Down pada tombol Run dan Jump.
    public void BeginLookFromButton(BaseEventData eventData)
    {
        if (!(eventData is PointerEventData pointerData))
            return;

        // Jangan mengambil alih pointer kamera yang sedang aktif.
        if (activePointerId != int.MinValue)
            return;

        activePointerId = pointerData.pointerId;
        lastPosition = pointerData.position;
        trackingButtonPointer = true;
    }

    // Hubungkan ke Pointer Up pada tombol Run dan Jump.
    public void EndLookFromButton(BaseEventData eventData)
    {
        if (!(eventData is PointerEventData pointerData))
            return;

        if (!trackingButtonPointer ||
            pointerData.pointerId != activePointerId)
            return;

        ResetTracking();
    }

    private void Update()
    {
        if (!trackingButtonPointer ||
            activePointerId == int.MinValue)
            return;

        if (Touchscreen.current == null)
        {
            ResetTracking();
            return;
        }

        // Cari jari yang memulai sentuhan pada tombol.
        foreach (var touch in Touchscreen.current.touches)
        {
            if (touch.touchId.ReadValue() != activePointerId)
                continue;

            if (!touch.press.isPressed)
            {
                ResetTracking();
                return;
            }

            Vector2 currentPosition =
                touch.position.ReadValue();

            lookDelta += currentPosition - lastPosition;
            lastPosition = currentPosition;

            return;
        }

        // Jangan reset hanya karena sentuhan belum ditemukan
        // pada frame ini. Pointer Up akan mengakhiri tracking.
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
