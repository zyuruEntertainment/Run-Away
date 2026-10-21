
using UnityEngine;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    public int maxHP = 100;
    public int currentHP = 100;

    public GameObject jumpscarePanel;
    public AudioSource jumpscareAudio;
    public GameObject gameOverPanel;

    public MonoBehaviour[] playerControls;

    private bool isDead;
    public bool IsDead => isDead;

    void Start()
    {
        currentHP = maxHP;

        if (jumpscarePanel != null)
            jumpscarePanel.SetActive(false);

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
    }

    public void TakeDamage(int damage)
    {
        if (isDead || damage <= 0) return;

        currentHP = Mathf.Max(0, currentHP - damage);

        if (currentHP <= 0)
            StartCoroutine(GameOverSequence());
    }

    IEnumerator GameOverSequence()
    {
        isDead = true;

        foreach (MonoBehaviour control in playerControls)
        {
            if (control != null)
                control.enabled = false;
        }

        AudioSource[] allAudioSources =
        FindObjectsByType<AudioSource>(
            FindObjectsInactive.Include
        );

        foreach (AudioSource source in allAudioSources)
        {
            if (source != null)
                source.Stop();
        }

        if (jumpscarePanel != null)
            jumpscarePanel.SetActive(true);

        if (jumpscareAudio != null)
        {
            jumpscareAudio.Stop();
            jumpscareAudio.loop = false;
            jumpscareAudio.Play();
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Time.timeScale = 1f;

        yield return new WaitForSecondsRealtime(3f);

        if (jumpscarePanel != null)
            jumpscarePanel.SetActive(false);

        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

        if (jumpscareAudio != null)
            jumpscareAudio.Stop();

        Time.timeScale = 0f;
    }
}
