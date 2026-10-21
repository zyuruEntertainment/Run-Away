using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(AudioSource))]
public class FootstepSound : MonoBehaviour
{
    public FPSController player;
    public AudioClip[] footstepSounds;

    public float baseInterval = 0.5f;
    public float minInterval = 0.22f;
    public float speedReference = 6f;
    public float movementThreshold = 0.15f;
    public float volume = 0.8f;

    private CharacterController controller;
    private AudioSource audioSource;
    private float stepTimer;
    private int lastSoundIndex = -1;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        audioSource = GetComponent<AudioSource>();

        audioSource.playOnAwake = false;
        audioSource.loop = false;
        audioSource.volume = volume;
    }

    void Start()
    {
        if (player == null)
            player = GetComponent<FPSController>();
    }

    void Update()
    {
        if (player == null ||
            footstepSounds == null ||
            footstepSounds.Length == 0)
        {
            return;
        }

        float speed = player.moveSpeed;

        Vector3 velocity = controller.velocity;
        velocity.y = 0f;

        bool moving = velocity.magnitude > movementThreshold;
        bool grounded = controller.isGrounded;

        if (!moving || !grounded)
        {
            stepTimer = 0f;
            return;
        }

        float speedRatio = Mathf.Max(
            speed / Mathf.Max(speedReference, 0.01f),
            0.01f
        );

        float interval = Mathf.Max(
            baseInterval / speedRatio,
            minInterval
        );

        stepTimer += Time.deltaTime;

        if (stepTimer >= interval)
        {
            PlayFootstep();
            stepTimer = 0f;
        }
    }

    void PlayFootstep()
    {
        if (footstepSounds.Length == 0)
            return;

        int index = Random.Range(0, footstepSounds.Length);

        if (footstepSounds.Length > 1)
        {
            while (index == lastSoundIndex)
                index = Random.Range(0, footstepSounds.Length);
        }

        lastSoundIndex = index;

        AudioClip clip = footstepSounds[index];

        if (clip != null)
            audioSource.PlayOneShot(clip, volume);
    }
}
