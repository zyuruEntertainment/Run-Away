
using UnityEngine;

public static class RunAwayProgress
{
    private const string DurationKey = "RunAway_Duration";
    private const string BestInfinityKey = "RunAway_BestInfinity";

    public static int SelectedDuration
    {
        get => PlayerPrefs.GetInt(DurationKey, 3);
        set
        {
            PlayerPrefs.SetInt(DurationKey, value);
            PlayerPrefs.Save();
        }
    }

    // 0 = infinity, 3, 5, 10 = countdown
    public static bool IsInfinity => SelectedDuration == 0;

    public static void UnlockAchievement(int minutes)
    {
        PlayerPrefs.SetInt(
            $"RunAway_Achievement_{minutes}m", 1);

        PlayerPrefs.Save();
    }

    public static bool IsAchievementUnlocked(int minutes)
    {
        return PlayerPrefs.GetInt(
            $"RunAway_Achievement_{minutes}m", 0) == 1;
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
