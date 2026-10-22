
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DurationSelectController : MonoBehaviour
{
    [Header("Duration Toggles")]
    public Toggle toggle3m;
    public Toggle toggle5m;
    public Toggle toggle10m;
    public Toggle toggleInfinity;

    private ToggleGroup toggleGroup;

    private void Awake()
    {
        toggleGroup = toggle3m != null
            ? toggle3m.group
            : null;

        if (toggleGroup == null)
        {
            Debug.LogError(
                "ToggleGroup belum dipasang pada toggle 3m!");
            return;
        }

        toggleGroup.allowSwitchOff = false;

        toggle3m.group = toggleGroup;
        toggle5m.group = toggleGroup;
        toggle10m.group = toggleGroup;
        toggleInfinity.group = toggleGroup;

        // Default selalu 3 menit saat membuka scene.
        // Hapus blok ini jika ingin pilihan terakhir diingat.
        toggle3m.SetIsOnWithoutNotify(true);
        toggle5m.SetIsOnWithoutNotify(false);
        toggle10m.SetIsOnWithoutNotify(false);
        toggleInfinity.SetIsOnWithoutNotify(false);
    }

    public void StartGame()
    {
        if (toggle3m.isOn)
            RunAwayProgress.SelectedDuration = 3;
        else if (toggle5m.isOn)
            RunAwayProgress.SelectedDuration = 5;
        else if (toggle10m.isOn)
            RunAwayProgress.SelectedDuration = 10;
        else if (toggleInfinity.isOn)
            RunAwayProgress.SelectedDuration = 0;
        else
        {
            Debug.LogError("Pilih durasi terlebih dahulu!");
            return;
        }

        Time.timeScale = 1f;
        SceneManager.LoadScene("Gameplay");
    }
}
