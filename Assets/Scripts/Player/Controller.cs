
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class FPSController : MonoBehaviour
{
    [Header("Camera")]
    public Transform playerCamera;
    public float mouseSensitivity = 2f;
    public float minLookAngle = -85f;
    public float maxLookAngle = 85f;

    [Header("Mobile Controls")]
    public MobileJoystick movementJoystick;
    public MobileLookArea cameraLookArea;

    [Header("Movement")]
    public float walkSpeed = 6f;
    public float moveSpeed = 6f;
    public float groundAcceleration = 25f;
    public float airAcceleration = 2f;

    [Header("Jump & Gravity")]
    public float jumpHeight = 2f;
    public float gravity = -20f;

    private CharacterController controller;
    private Vector3 horizontalVelocity;
    private float verticalVelocity;
    private float cameraPitch;
    private bool isPaused;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();

        moveSpeed = walkSpeed;
        mouseSensitivity = GameSettings.MouseSensitivity;

        if (playerCamera != null)
        {
            cameraPitch = playerCamera.localEulerAngles.x;

            if (cameraPitch > 180f)
                cameraPitch -= 360f;
        }

        if (!Application.isMobilePlatform)
            LockCursor();
    }

    private void Update()
    {
        mouseSensitivity = GameSettings.MouseSensitivity;

        if (Time.timeScale == 0f)
        {
            isPaused = true;
            return;
        }

        if (isPaused)
        {
            isPaused = false;

            if (!Application.isMobilePlatform)
                LockCursor();
        }

        HandleCursor();
        Look();
        Move();
    }

    private void HandleCursor()
    {
        if (Application.isMobilePlatform)
            return;

        if (Keyboard.current != null &&
            Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    private void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Look()
    {
        if (playerCamera == null)
            return;

        Vector2 lookDelta = Vector2.zero;

        // Prioritaskan input sentuh jika tersedia.
        if (cameraLookArea != null)
            lookDelta = cameraLookArea.ConsumeDelta();

        // Kamera HP: geser jari di area look.
        if (lookDelta.sqrMagnitude > 0f)
        {
            ApplyLook(lookDelta);
            return;
        }

        // Kamera PC: gunakan mouse.
        if (Application.isMobilePlatform)
            return;

        if (Mouse.current == null ||
            Cursor.lockState != CursorLockMode.Locked)
            return;

        Vector2 mouseDelta = Mouse.current.delta.ReadValue();
        ApplyLook(mouseDelta);
    }

    private void ApplyLook(Vector2 delta)
    {
        float mouseX = delta.x * mouseSensitivity * 0.05f;
        float mouseY = delta.y * mouseSensitivity * 0.05f;

        transform.Rotate(Vector3.up * mouseX);

        cameraPitch -= mouseY;
        cameraPitch = Mathf.Clamp(
            cameraPitch,
            minLookAngle,
            maxLookAngle
        );

        playerCamera.localRotation =
            Quaternion.Euler(cameraPitch, 0f, 0f);
    }

    private void Move()
    {
        bool grounded = controller.isGrounded;

        if (grounded && verticalVelocity < 0f)
            verticalVelocity = -2f;

        float x = 0f;
        float z = 0f;

        // Input keyboard untuk PC.
        if (Keyboard.current != null)
        {
            if (Keyboard.current.aKey.isPressed) x -= 1f;
            if (Keyboard.current.dKey.isPressed) x += 1f;
            if (Keyboard.current.sKey.isPressed) z -= 1f;
            if (Keyboard.current.wKey.isPressed) z += 1f;
        }

        Vector2 input = new Vector2(x, z);

        // Joystick menggantikan input keyboard saat digerakkan.
        if (movementJoystick != null &&
            movementJoystick.InputDirection.sqrMagnitude > 0.001f)
        {
            input = movementJoystick.InputDirection;
        }

        input = Vector2.ClampMagnitude(input, 1f);

        Vector3 direction =
            transform.right * input.x +
            transform.forward * input.y;

        Vector3 targetVelocity = direction * moveSpeed;

        float acceleration = grounded
            ? groundAcceleration
            : airAcceleration;

        horizontalVelocity = Vector3.MoveTowards(
            horizontalVelocity,
            targetVelocity,
            acceleration * Time.deltaTime
        );

        // Jump tetap tersedia untuk keyboard.
        if (grounded &&
            Keyboard.current != null &&
            Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            verticalVelocity = Mathf.Sqrt(
                jumpHeight * -2f * gravity
            );
        }

        verticalVelocity += gravity * Time.deltaTime;

        Vector3 finalVelocity = horizontalVelocity;
        finalVelocity.y = verticalVelocity;

        controller.Move(finalVelocity * Time.deltaTime);
    }

    private void OnDisable()
    {
        if (!Application.isMobilePlatform)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}
