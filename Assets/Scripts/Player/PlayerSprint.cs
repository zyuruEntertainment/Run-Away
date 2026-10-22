
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

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

    [Header("Audio")]
    public AudioSource panicAudioSource;
    public AudioSource breathingAudioSource;
    public AudioClip panicSound;
    public AudioClip heavyBreathingSound;

    public float CurrentStamina { get; private set; }

    public float StaminaPercent => maxStamina > 0f
        ? CurrentStamina / maxStamina
        : 0f;

    public bool IsSprinting { get; private set; }

    private float regenTimer;
    private FPSController movement;
    private bool breathingPlayed;
    private bool mobileSprintHeld;

    private bool IsMobile =>
        ControlModeSettings.CurrentMode == ControlMode.Mobile;

    private void Awake()
    {
        movement = GetComponent<FPSController>();
        CurrentStamina = maxStamina;

        if (panicAudioSource != null)
        {
            panicAudioSource.playOnAwake = false;
            panicAudioSource.loop = false;
        }

        if (breathingAudioSource != null)
        {
            breathingAudioSource.playOnAwake = false;
            breathingAudioSource.loop = false;
        }
    }

    // Dipanggil saat tombol sprint ditekan.
    public void StartSprint()
    {
        mobileSprintHeld = true;
    }

    // Dipanggil saat tombol sprint dilepas.
    public void StopSprint()
    {
        mobileSprintHeld = false;
    }

    private void Update()
    {
        if (Time.timeScale == 0f)
        {
            IsSprinting = false;

            if (movement != null)
                movement.moveSpeed = movement.walkSpeed;

            return;
        }

        bool keyboardSprint =
            Keyboard.current != null &&
            Keyboard.current[sprintKey].isPressed;

        bool keyboardMoving =
            Keyboard.current != null &&
            (Keyboard.current.wKey.isPressed ||
             Keyboard.current.aKey.isPressed ||
             Keyboard.current.sKey.isPressed ||
             Keyboard.current.dKey.isPressed);

        bool moving;

        if (IsMobile)
        {
            // Periksa input joystick mobile.
            moving =
                movement != null &&
                movement.movementJoystick != null &&
                movement.movementJoystick.InputDirection.sqrMagnitude > 0.01f;
        }
        else
        {
            moving = keyboardMoving;
        }

        bool sprintHeld = IsMobile
            ? mobileSprintHeld
            : keyboardSprint;

        bool wasSprinting = IsSprinting;

        IsSprinting =
            sprintHeld &&
            moving &&
            CurrentStamina > 0f;

        if (IsSprinting)
        {
            CurrentStamina -= staminaDrain * Time.deltaTime;
            CurrentStamina = Mathf.Max(0f, CurrentStamina);
            regenTimer = regenDelay;

            if (CurrentStamina <= 0f)
                IsSprinting = false;
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

        // Suara panik saat mulai sprint.
        if (IsSprinting && !wasSprinting)
        {
            if (panicAudioSource != null && panicSound != null)
            {
                panicAudioSource.Stop();
                panicAudioSource.clip = panicSound;
                panicAudioSource.loop = false;
                panicAudioSource.Play();
            }
        }

        // Hentikan suara panik saat sprint berhenti.
        if (!IsSprinting && wasSprinting)
        {
            if (panicAudioSource != null)
            {
                panicAudioSource.Stop();
                panicAudioSource.clip = null;
            }
        }

        // Suara napas berat saat stamina habis.
        if (CurrentStamina <= 0f && !breathingPlayed)
        {
            if (breathingAudioSource != null &&
                heavyBreathingSound != null)
            {
                breathingAudioSource.Stop();
                breathingAudioSource.clip = heavyBreathingSound;
                breathingAudioSource.loop = false;
                breathingAudioSource.Play();
            }

            breathingPlayed = true;
        }

        // Izinkan suara napas diputar lagi setelah stamina pulih.
        if (CurrentStamina > 0f)
        {
            breathingPlayed = false;
        }

        // Terapkan kecepatan ke FPSController.
        if (movement != null)
        {
            movement.moveSpeed = IsSprinting
                ? sprintSpeed
                : movement.walkSpeed;
        }
    }

    private void OnDisable()
    {
        mobileSprintHeld = false;
        IsSprinting = false;

        if (movement != null)
            movement.moveSpeed = movement.walkSpeed;

        if (panicAudioSource != null)
            panicAudioSource.Stop();
    }
}
