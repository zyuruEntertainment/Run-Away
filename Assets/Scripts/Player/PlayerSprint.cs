using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerSprint : MonoBehaviour
{
    [Header("Sprint")]
    public float sprintSpeed = 10f;
    public Key sprintKey = Key.LeftShift;

    [Header("Stamina")]
    public float maxStamina = 100f;
    public float staminaDrain = 25f;
    public float staminaRegen = 15f;
    public float regenDelay = 1f;

    public float CurrentStamina { get; private set; }
    public float StaminaPercent => maxStamina > 0f
        ? CurrentStamina / maxStamina
        : 0f;

    public bool IsSprinting { get; private set; }

    private float regenTimer;
    private FPSController movement;

    private void Awake()
    {
        movement = GetComponent<FPSController>();
        CurrentStamina = maxStamina;
    }

    private void Update()
    {
        bool sprintHeld =
            Keyboard.current != null &&
            Keyboard.current[sprintKey].isPressed;

        bool moving =
            Keyboard.current != null &&
            (Keyboard.current.wKey.isPressed ||
             Keyboard.current.aKey.isPressed ||
             Keyboard.current.sKey.isPressed ||
             Keyboard.current.dKey.isPressed);

        IsSprinting =
            sprintHeld &&
            moving &&
            CurrentStamina > 0f;

        if (IsSprinting)
        {
            CurrentStamina -= staminaDrain * Time.deltaTime;
            CurrentStamina = Mathf.Max(0f, CurrentStamina);
            regenTimer = regenDelay;
        }
        else
        {
            regenTimer -= Time.deltaTime;

            if (regenTimer <= 0f)
            {
                CurrentStamina += staminaRegen * Time.deltaTime;
                CurrentStamina = Mathf.Min(maxStamina, CurrentStamina);
            }
        }

        if (CurrentStamina <= 0f)
        {
            IsSprinting = false;
        }

        if (movement != null)
        {
            movement.moveSpeed = IsSprinting
                ? sprintSpeed
                : movement.walkSpeed;
        }
    }
}
