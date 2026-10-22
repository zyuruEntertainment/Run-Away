
using UnityEngine;
using UnityEngine.EventSystems;

public class MobileJoystick : MonoBehaviour,
    IPointerDownHandler,
    IDragHandler,
    IPointerUpHandler
{
    [Header("Joystick")]
    public RectTransform handle;

    [Tooltip("Batas gerak handle di dalam lingkaran joystick.")]
    [Range(0.1f, 1f)]
    public float radiusMultiplier = 1f;

    public Vector2 InputDirection { get; private set; }

    private RectTransform joystickRect;
    private Canvas canvas;

    private int activePointerId = int.MinValue;
    private float travelRadius;

    private void Awake()
    {
        canvas = GetComponentInParent<Canvas>();

        // Gunakan lingkaran tempat handle berada,
        // bukan RectTransform root yang ukurannya nol.
        if (handle != null)
        {
            joystickRect = handle.parent as RectTransform;

            // Handle selalu bergerak dari titik tengah.
            handle.anchorMin = new Vector2(0.5f, 0.5f);
            handle.anchorMax = new Vector2(0.5f, 0.5f);
            handle.anchoredPosition = Vector2.zero;
        }
        else
        {
            joystickRect = GetComponent<RectTransform>();

            Debug.LogError(
                "MobileJoystick: Handle belum dihubungkan!",
                this
            );
        }

        CalculateRadius();
        ResetJoystick();
    }

    private void CalculateRadius()
    {
        if (joystickRect == null || handle == null)
        {
            travelRadius = 0f;
            return;
        }

        float baseRadius = Mathf.Min(
            joystickRect.rect.width,
            joystickRect.rect.height
        ) * 0.5f;

        // Sisakan ruang agar tepi handle tetap di dalam lingkaran.
        float handleRadiusX =
            handle.rect.width * Mathf.Abs(handle.localScale.x) * 0.5f;

        float handleRadiusY =
            handle.rect.height * Mathf.Abs(handle.localScale.y) * 0.5f;

        float handleRadius = Mathf.Max(
            handleRadiusX,
            handleRadiusY
        );

        travelRadius = Mathf.Max(
            0f,
            baseRadius - handleRadius
        ) * radiusMultiplier;

        if (travelRadius <= 0f)
        {
            Debug.LogWarning(
                "MobileJoystick: Radius gerak nol. " +
                "Periksa ukuran lingkaran dan handle.",
                this
            );
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (activePointerId != int.MinValue)
            return;

        activePointerId = eventData.pointerId;
        UpdateJoystick(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (eventData.pointerId != activePointerId)
            return;

        UpdateJoystick(eventData);
    }

    private void UpdateJoystick(PointerEventData eventData)
    {
        if (joystickRect == null ||
            handle == null ||
            travelRadius <= 0f)
        {
            return;
        }

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

        // Hitung posisi sentuhan relatif terhadap pusat lingkaran.
        Vector2 centerOffset = new Vector2(
            (0.5f - joystickRect.pivot.x) *
            joystickRect.rect.width,

            (0.5f - joystickRect.pivot.y) *
            joystickRect.rect.height
        );

        Vector2 offset = localPoint - centerOffset;

        // Batasi posisi handle sesuai radius maksimum.
        Vector2 clampedOffset = Vector2.ClampMagnitude(
            offset,
            travelRadius
        );

        handle.anchoredPosition = clampedOffset;

        // Hasil input berada pada rentang -1 sampai 1.
        InputDirection = clampedOffset / travelRadius;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.pointerId != activePointerId)
            return;

        ResetJoystick();
    }

    private void ResetJoystick()
    {
        InputDirection = Vector2.zero;
        activePointerId = int.MinValue;

        if (handle != null)
            handle.anchoredPosition = Vector2.zero;
    }

    private void OnDisable()
    {
        ResetJoystick();
    }
}
