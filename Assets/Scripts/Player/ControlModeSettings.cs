
using UnityEngine;

public enum ControlMode
{
    Desktop,
    Mobile
}

public static class ControlModeSettings
{
    private const string Key = "ControlMode";

    public static ControlMode CurrentMode
    {
        get
        {
            return (ControlMode)PlayerPrefs.GetInt(
                Key,
                (int)ControlMode.Desktop
            );
        }
    }

    public static void SetMode(ControlMode mode)
    {
        PlayerPrefs.SetInt(Key, (int)mode);
        PlayerPrefs.Save();
    }
}
