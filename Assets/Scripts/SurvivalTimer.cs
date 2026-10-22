
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class SurvivalTimer : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text minutesText;
    public TMP_Text secondsText;

    [Header("Player")]
    public PlayerHealth playerHealth;

    private float elapsedTime;
    private float remainingTime;
    private bool isFinished;
    private bool isInfinity;

    private void Start()
    {
        Time.timeScale = 1f;

        int duration = RunAwayProgress.SelectedDuration;

        isInfinity = duration == 0;
        elapsedTime = 0f;
        remainingTime = duration * 60f;

        Debug.Log(
            "Timer memulai mode: " +
            (isInfinity ? "Infinity" : duration + " menit")
        );

        UpdateDisplay();
    }


    private void Update()
    {
        if (isFinished)
            return;

        // Hentikan timer tepat saat pemain mati.
        if (playerHealth != null && playerHealth.IsDead)
        {
            EndByGameOver();
            return;
        }

        elapsedTime += Time.deltaTime;

        if (!isInfinity)
        {
            remainingTime -= Time.deltaTime;

            if (remainingTime <= 0f)
            {
                remainingTime = 0f;
                UpdateDisplay();
                WinGame();
                return;
            }
        }

        UpdateDisplay();
    }

    private void UpdateDisplay()
    {
        float displayTime = isInfinity
            ? elapsedTime
            : remainingTime;

        int totalSeconds = Mathf.Max(
            0, Mathf.FloorToInt(displayTime));

        int minutes = totalSeconds / 60;
        int seconds = totalSeconds % 60;

        if (minutesText != null)
            minutesText.text = minutes.ToString("00");

        if (secondsText != null)
            secondsText.text = seconds.ToString("00");
    }

    private void WinGame()
    {
        if (isFinished)
            return;

        isFinished = true;

        // Rekor infinity hanya diperbarui pada mode infinity.
        if (isInfinity)
            RunAwayProgress.SaveInfinityRecord(elapsedTime);

        // Achievement durasi yang berhasil diselesaikan.

        if (!isInfinity)
        {
            int completedMinutes = RunAwayProgress.SelectedDuration;

            if (completedMinutes >= 3)
                RunAwayProgress.UnlockAchievement(3);

            if (completedMinutes >= 5)
                RunAwayProgress.UnlockAchievement(5);

            if (completedMinutes >= 10)
                RunAwayProgress.UnlockAchievement(10);
        }


        Time.timeScale = 1f;
        SceneManager.LoadScene("Win");
    }

    private void EndByGameOver()
    {
        if (isFinished)
            return;

        isFinished = true;

        // Hanya mode infinity yang mencatat rekor dari game over.
        if (isInfinity)
            RunAwayProgress.SaveInfinityRecord(elapsedTime);
    }
}
