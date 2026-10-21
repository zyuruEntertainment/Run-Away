
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(AudioSource))]
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
    public AudioClip panicSound;
    public AudioClip heavyBreathingSound;

    public float CurrentStamina { get; private set; }

    public float StaminaPercent => maxStamina > 0f
        ? CurrentStamina / maxStamina
        : 0f;

    public bool IsSprinting { get; private set; }

    private float regenTimer;
    private FPSController movement;
    private AudioSource audioSource;
    private bool breathingPlayed;

    private void Awake()
    {
        movement = GetComponent<FPSController>();
        audioSource = GetComponent<AudioSource>();
        CurrentStamina = maxStamina;

        audioSource.playOnAwake = false;
        audioSource.loop = false;
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

        if (IsSprinting && !wasSprinting)
        {
            if (panicSound != null)
            {
                audioSource.Stop();
                audioSource.clip = panicSound;
                audioSource.loop = false;
                audioSource.Play();
            }
        }

        if (!IsSprinting && audioSource.clip == panicSound)
        {
            audioSource.Stop();
            audioSource.clip = null;
        }

        if (CurrentStamina <= 0f && !breathingPlayed)
        {
            if (heavyBreathingSound != null)
            {
                audioSource.PlayOneShot(heavyBreathingSound);
            }

            breathingPlayed = true;
        }

        if (CurrentStamina > 0f)
        {
            breathingPlayed = false;
        }

        if (movement != null)
        {
            movement.moveSpeed = IsSprinting
                ? sprintSpeed
                : movement.walkSpeed;
        }
    }
}
