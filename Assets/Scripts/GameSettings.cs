using UnityEngine;

public static class GameSettings
{
    private const string SensitivityKey = "MouseSensitivity";

    public static float MouseSensitivity
    {
        get => PlayerPrefs.GetFloat(SensitivityKey, 2f);

        set
        {
            PlayerPrefs.SetFloat(SensitivityKey, value);
            PlayerPrefs.Save();
        }
    }
}
