
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(AudioSource))]
public class MonsterFootstepSound : MonoBehaviour
{
    public Transform player;
    public PlayerHealth playerHealth;
    public AudioClip monsterSound;

    public float minDistance = 1f;
    public float maxDistance = 50f;

    public float farVolume = 0.15f;
    public float nearVolume = 1f;

    public float farPitch = 0.7f;
    public float nearPitch = 2f;

    public float pitchSmoothness = 3f;
    public bool playOnStart = true;

    private AudioSource audioSource;
    private bool soundStopped;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        audioSource.playOnAwake = false;
        audioSource.loop = true;
        audioSource.spatialBlend = 1f;
        audioSource.dopplerLevel = 0f;
        audioSource.rolloffMode = AudioRolloffMode.Linear;
    }

    void Start()
    {
        if (player == null)
        {
            MonsterChase chase = GetComponent<MonsterChase>();

            if (chase != null)
                player = chase.player;
        }

        if (playerHealth == null && player != null)
            playerHealth = player.GetComponent<PlayerHealth>();

        if (monsterSound != null)
        {
            audioSource.clip = monsterSound;
            audioSource.loop = true;

            if (playOnStart &&
                (playerHealth == null || playerHealth.currentHP > 0))
            {
                audioSource.Play();
            }
        }
    }

    void Update()
    {
        if (soundStopped)
            return;

        if (playerHealth != null && playerHealth.currentHP <= 0)
        {
            StopMonsterSound();
            return;
        }

        if (player == null || monsterSound == null)
            return;

        float distance = Vector3.Distance(
            transform.position,
            player.position
        );

        float proximity = Mathf.InverseLerp(
            maxDistance,
            minDistance,
            distance
        );

        audioSource.minDistance = minDistance;
        audioSource.maxDistance = maxDistance;

        audioSource.volume = Mathf.Lerp(
            farVolume,
            nearVolume,
            proximity
        );

        float targetPitch = Mathf.Lerp(
            farPitch,
            nearPitch,
            proximity
        );

        audioSource.pitch = Mathf.Lerp(
            audioSource.pitch,
            targetPitch,
            pitchSmoothness * Time.deltaTime
        );

        if (audioSource.clip != monsterSound)
        {
            audioSource.clip = monsterSound;
            audioSource.loop = true;
        }

        if (playOnStart && !audioSource.isPlaying)
            audioSource.Play();
    }

    void StopMonsterSound()
    {
        soundStopped = true;
        audioSource.Stop();
    }

    void OnDisable()
    {
        if (audioSource != null)
            audioSource.Stop();
    }
}
