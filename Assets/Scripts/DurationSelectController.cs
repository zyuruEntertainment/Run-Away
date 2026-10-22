
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

    [Header("Toggle Group")]
    public ToggleGroup toggleGroup;

    private bool initialized;

    private void Awake()
    {
        // Validasi semua referensi dari Inspector.
        if (toggle3m == null ||
            toggle5m == null ||
            toggle10m == null ||
            toggleInfinity == null)
        {
            Debug.LogError(
                "DurationSelectController: Assign keempat Toggle di Inspector!",
                this
            );
            enabled = false;
            return;
        }

        // Ambil ToggleGroup dari Inspector atau Toggle 3m.
        if (toggleGroup == null)
            toggleGroup = toggle3m.group;

        if (toggleGroup == null)
        {
            Debug.LogError(
                "ToggleGroup belum di-assign. Hubungkan ToggleGroup di Inspector.",
                this
            );
            enabled = false;
            return;
        }

        toggleGroup.allowSwitchOff = false;

        // Pastikan keempat Toggle berada dalam grup yang sama.
        toggle3m.group = toggleGroup;
        toggle5m.group = toggleGroup;
        toggle10m.group = toggleGroup;
        toggleInfinity.group = toggleGroup;

        // Pasang listener sebelum memulihkan pilihan tersimpan.
        toggle3m.onValueChanged.AddListener(On3mChanged);
        toggle5m.onValueChanged.AddListener(On5mChanged);
        toggle10m.onValueChanged.AddListener(On10mChanged);
        toggleInfinity.onValueChanged.AddListener(OnInfinityChanged);

        // Pulihkan pilihan terakhir, bukan selalu memaksa 3m.
        int savedDuration = RunAwayProgress.SelectedDuration;

        toggle3m.SetIsOnWithoutNotify(savedDuration == 3);
        toggle5m.SetIsOnWithoutNotify(savedDuration == 5);
        toggle10m.SetIsOnWithoutNotify(savedDuration == 10);
        toggleInfinity.SetIsOnWithoutNotify(savedDuration == 0);

        // Pastikan grup mengenali kondisi awalnya.
        toggleGroup.EnsureValidState();

        initialized = true;

        Debug.Log("Pilihan durasi dipulihkan: " + savedDuration);
    }

    private void OnDestroy()
    {
        if (toggle3m != null)
            toggle3m.onValueChanged.RemoveListener(On3mChanged);

        if (toggle5m != null)
            toggle5m.onValueChanged.RemoveListener(On5mChanged);

        if (toggle10m != null)
            toggle10m.onValueChanged.RemoveListener(On10mChanged);

        if (toggleInfinity != null)
            toggleInfinity.onValueChanged.RemoveListener(OnInfinityChanged);
    }

    private void On3mChanged(bool isOn)
    {
        if (initialized && isOn)
            SaveDuration(3);
    }

    private void On5mChanged(bool isOn)
    {
        if (initialized && isOn)
            SaveDuration(5);
    }

    private void On10mChanged(bool isOn)
    {
        if (initialized && isOn)
            SaveDuration(10);
    }

    private void OnInfinityChanged(bool isOn)
    {
        if (initialized && isOn)
            SaveDuration(0);
    }

    private void SaveDuration(int duration)
    {
        RunAwayProgress.SelectedDuration = duration;

        Debug.Log(
            "Pilihan Toggle berubah dan tersimpan: " +
            (duration == 0 ? "Infinity" : duration + " menit")
        );
    }

    public void StartGame()
    {
        // Baca kondisi Toggle terbaru sebelum pindah scene.
        if (toggle3m.isOn)
            SaveDuration(3);
        else if (toggle5m.isOn)
            SaveDuration(5);
        else if (toggle10m.isOn)
            SaveDuration(10);
        else if (toggleInfinity.isOn)
            SaveDuration(0);
        else
        {
            Debug.LogError("Tidak ada durasi yang dipilih!");
            return;
        }

        Time.timeScale = 1f;

        // Gunakan sistem loading dari repo Run-Away.
        SceneLoader.TargetSceneName = "Gameplay";
        SceneManager.LoadScene("loading");
    }
}
