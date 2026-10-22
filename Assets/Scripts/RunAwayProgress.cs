
using UnityEngine;

public static class RunAwayProgress
{
    private const string DurationKey = "RunAway_Duration";
    private const string BestInfinityKey = "RunAway_BestInfinity";

    // 0 = Infinity, 3 = 3 menit, 5 = 5 menit, 10 = 10 menit.
    public static int SelectedDuration
    {
        get
        {
            int duration = PlayerPrefs.GetInt(DurationKey, 3);

            if (duration != 0 && duration != 3 &&
                duration != 5 && duration != 10)
            {
                duration = 3;
            }

            return duration;
        }
        set
        {
            if (value != 0 && value != 3 &&
                value != 5 && value != 10)
            {
                Debug.LogWarning("Durasi tidak valid: " + value);
                return;
            }

            PlayerPrefs.SetInt(DurationKey, value);
            PlayerPrefs.Save();

            Debug.Log("Durasi tersimpan: " +
                (value == 0 ? "Infinity" : value + " menit"));
        }
    }

    public static bool IsInfinity => SelectedDuration == 0;

    public static void UnlockAchievement(int minutes)
    {
        PlayerPrefs.SetInt("RunAway_Achievement_" + minutes + "m", 1);
        PlayerPrefs.Save();
    }

    public static bool IsAchievementUnlocked(int minutes)
    {
        return PlayerPrefs.GetInt(
            "RunAway_Achievement_" + minutes + "m", 0) == 1;
    }

    public static float BestInfinityTime
    {
        get => PlayerPrefs.GetFloat(BestInfinityKey, 0f);
    }

    public static void SaveInfinityRecord(float seconds)
    {
        if (seconds <= BestInfinityTime)
            return;

        PlayerPrefs.SetFloat(BestInfinityKey, seconds);
        PlayerPrefs.Save();
    }

    public static string FormatTime(float totalSeconds)
    {
        int seconds = Mathf.Max(0, Mathf.FloorToInt(totalSeconds));
        int minutes = seconds / 60;
        int remainingSeconds = seconds % 60;

        return $"{minutes:00}:{remainingSeconds:00}";
    }
}
