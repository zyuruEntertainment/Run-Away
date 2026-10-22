
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

    [Header("Movement")]
    public float walkSpeed = 6f;
    public float moveSpeed = 6f;
    public float groundAcceleration = 25f;
    public float airAcceleration = 2f;
    public float jumpHeight = 2f;
    public float gravity = -20f;

    [Header("Mobile Controls")]
    public MobileJoystick movementJoystick;
    public MobileLookArea cameraLookArea;
    public GameObject mobileControlsRoot;

    private CharacterController controller;
    private Vector3 horizontalVelocity;
    private float verticalVelocity;
    private float cameraPitch;
    private bool isPaused;
    private bool wasMobile;

    // Permintaan lompat dari tombol UI.
    private bool jumpRequested;

    private bool IsMobile =>
        ControlModeSettings.CurrentMode == ControlMode.Mobile;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        moveSpeed = walkSpeed;
        mouseSensitivity = GameSettings.MouseSensitivity;

        ApplyControlMode();
    }

    private void Update()
    {
        if (Time.timeScale == 0f)
        {
            isPaused = true;
            return;
        }

        if (isPaused)
        {
            isPaused = false;
            ApplyControlMode();
        }

        // Periksa jika mode kontrol berubah.
        if (wasMobile != IsMobile)
        {
            ApplyControlMode();
        }

        mouseSensitivity = GameSettings.MouseSensitivity;

        if (IsMobile)
        {
            LookMobile();
        }
        else
        {
            HandleCursor();
            LookDesktop();
        }

        Move();
    }

    private void ApplyControlMode()
    {
        wasMobile = IsMobile;

        if (mobileControlsRoot != null)
        {
            mobileControlsRoot.SetActive(IsMobile);
        }

        if (IsMobile)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            LockCursor();
        }
    }

    private void HandleCursor()
    {
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

    private void LookDesktop()
    {
        if (Mouse.current == null ||
            Cursor.lockState != CursorLockMode.Locked ||
            playerCamera == null)
        {
            return;
        }

        Vector2 mouseDelta = Mouse.current.delta.ReadValue();

        float mouseX = mouseDelta.x * mouseSensitivity * 0.05f;
        float mouseY = mouseDelta.y * mouseSensitivity * 0.05f;

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

    private void LookMobile()
    {
        if (playerCamera == null || cameraLookArea == null)
            return;

        Vector2 touchDelta = cameraLookArea.ConsumeDelta();

        float mouseX = touchDelta.x * mouseSensitivity * 0.05f;
        float mouseY = touchDelta.y * mouseSensitivity * 0.05f;

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

    // Dipanggil oleh Button UI melalui On Click().
    public void Jump()
    {
        if (controller == null)
            return;

        // Tidak menerima lompat saat pemain sedang di udara.
        if (!controller.isGrounded || verticalVelocity > 0f)
            return;

        jumpRequested = true;
    }

    private void Move()
    {
        bool grounded = controller.isGrounded;

        if (grounded && verticalVelocity < 0f)
        {
            verticalVelocity = -2f;
        }

        float x = 0f;
        float z = 0f;
        bool jumpPressed = false;

        if (IsMobile)
        {
            // Gerakan berasal dari joystick mobile.
            if (movementJoystick != null)
            {
                Vector2 joystickInput =
                    movementJoystick.InputDirection;

                x = joystickInput.x;
                z = joystickInput.y;
            }
        }
        else if (Keyboard.current != null)
        {
            // Kontrol desktop.
            if (Keyboard.current.aKey.isPressed) x -= 1f;
            if (Keyboard.current.dKey.isPressed) x += 1f;
            if (Keyboard.current.sKey.isPressed) z -= 1f;
            if (Keyboard.current.wKey.isPressed) z += 1f;

            jumpPressed =
                Keyboard.current.spaceKey.wasPressedThisFrame;
        }

        Vector3 input =
            transform.right * x +
            transform.forward * z;

        input = Vector3.ClampMagnitude(input, 1f);

        Vector3 targetVelocity = input * moveSpeed;

        float acceleration = grounded
            ? groundAcceleration
            : airAcceleration;

        horizontalVelocity = Vector3.MoveTowards(
            horizontalVelocity,
            targetVelocity,
            acceleration * Time.deltaTime
        );

        // Lompat dari Space atau tombol UI.
        if (grounded && (jumpPressed || jumpRequested))
        {
            verticalVelocity = Mathf.Sqrt(
                jumpHeight * -2f * gravity
            );

            jumpRequested = false;
        }

        // Batalkan permintaan jika pemain sudah tidak di tanah.
        else if (!grounded && jumpRequested)
        {
            jumpRequested = false;
        }

        verticalVelocity += gravity * Time.deltaTime;

        Vector3 finalVelocity = horizontalVelocity;
        finalVelocity.y = verticalVelocity;

        controller.Move(finalVelocity * Time.deltaTime);
    }

    private void OnDisable()
    {
        jumpRequested = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
