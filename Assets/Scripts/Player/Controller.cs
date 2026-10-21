using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class FPSController : MonoBehaviour
{
    public Transform playerCamera;
    public float mouseSensitivity = 2f;
    public float minLookAngle = -85f;
    public float maxLookAngle = 85f;

    public float walkSpeed = 6f;
    public float moveSpeed = 6f;
    public float groundAcceleration = 25f;
    public float airAcceleration = 2f;

    public float jumpHeight = 2f;
    public float gravity = -20f;

    private CharacterController controller;
    private Vector3 horizontalVelocity;
    private float verticalVelocity;
    private float cameraPitch;

    private void Awake()
    {
        moveSpeed = walkSpeed;
        controller = GetComponent<CharacterController>();
        mouseSensitivity = GameSettings.MouseSensitivity;
        LockCursor();
    }

    private void Update()
    {
        HandleCursor();
        Look();
        Move();
    }

    private void HandleCursor()
    {
        if (Keyboard.current != null &&
            Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        if (Mouse.current != null &&
            Mouse.current.leftButton.wasPressedThisFrame)
        {
            LockCursor();
        }
    }

    private void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Look()
    {
        if (Mouse.current == null ||
            Cursor.lockState != CursorLockMode.Locked ||
            playerCamera == null)
            return;

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

    private void Move()
    {
        bool grounded = controller.isGrounded;

        if (grounded && verticalVelocity < 0f)
        {
            verticalVelocity = -2f;
        }

        float x = 0f;
        float z = 0f;

        if (Keyboard.current != null)
        {
            if (Keyboard.current.aKey.isPressed) x -= 1f;
            if (Keyboard.current.dKey.isPressed) x += 1f;
            if (Keyboard.current.sKey.isPressed) z -= 1f;
            if (Keyboard.current.wKey.isPressed) z += 1f;
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
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
