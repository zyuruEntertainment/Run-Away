
using UnityEngine;
using TMPro;

public class MainMenuAchievements : MonoBehaviour
{
    [Header("Achievement Status")]
    public TMP_Text achievement3m;
    public TMP_Text achievement5m;
    public TMP_Text achievement10m;

    [Header("Infinity Best Record")]
    public TMP_Text bestMinutesText;
    public TMP_Text bestSecondsText;

    private void OnEnable()
    {
        RefreshAchievements();
    }

    public void RefreshAchievements()
    {
        SetStatus(achievement3m, 3);
        SetStatus(achievement5m, 5);
        SetStatus(achievement10m, 10);

        float bestTime = RunAwayProgress.BestInfinityTime;

        int totalSeconds = Mathf.Max(
            0, Mathf.FloorToInt(bestTime));

        int minutes = totalSeconds / 60;
        int seconds = totalSeconds % 60;

        if (bestMinutesText != null)
            bestMinutesText.text = minutes.ToString("00");

        if (bestSecondsText != null)
            bestSecondsText.text = seconds.ToString("00");
    }

    private void SetStatus(TMP_Text label, int minutes)
    {
        if (label == null)
            return;

        label.text = RunAwayProgress.IsAchievementUnlocked(minutes)
            ? "DONE"
            : "LOCKED";
    }
}
