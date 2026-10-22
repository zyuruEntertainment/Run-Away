
using UnityEngine;
using UnityEngine.UI;

public class ControlModeToggleUI : MonoBehaviour
{
    [Header("Control Mode Toggles")]
    public Toggle mobileToggle;
    public Toggle desktopToggle;

    private bool isInitializing;

    private void Start()
    {
        isInitializing = true;

        if (mobileToggle == null || desktopToggle == null)
        {
            Debug.LogError("Toggle Mobile atau Desktop belum dipasang!");
            return;
        }

        // Pastikan hanya satu toggle yang bisa aktif.
        ToggleGroup group = mobileToggle.group;

        if (group != null)
        {
            desktopToggle.group = group;
        }

        // Muat pilihan yang tersimpan.
        bool mobileSelected =
            ControlModeSettings.CurrentMode == ControlMode.Mobile;

        mobileToggle.SetIsOnWithoutNotify(mobileSelected);
        desktopToggle.SetIsOnWithoutNotify(!mobileSelected);

        mobileToggle.onValueChanged.AddListener(OnMobileChanged);
        desktopToggle.onValueChanged.AddListener(OnDesktopChanged);

        isInitializing = false;
    }

    private void OnMobileChanged(bool isOn)
    {
        if (isInitializing || !isOn) return;

        ControlModeSettings.SetMode(ControlMode.Mobile);
    }

    private void OnDesktopChanged(bool isOn)
    {
        if (isInitializing || !isOn) return;

        ControlModeSettings.SetMode(ControlMode.Desktop);
    }

    private void OnDestroy()
    {
        if (mobileToggle != null)
            mobileToggle.onValueChanged.RemoveListener(OnMobileChanged);

        if (desktopToggle != null)
            desktopToggle.onValueChanged.RemoveListener(OnDesktopChanged);
    }
}
