using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SensitivitySlider : MonoBehaviour
{
    public Slider slider;
    public TMP_Text sensitivityText;

    private void Start()
    {
        if (slider == null) return;

        slider.minValue = 0.1f;
        slider.maxValue = 10f;
        slider.wholeNumbers = false;

        slider.SetValueWithoutNotify(
            GameSettings.MouseSensitivity
        );

        UpdateSensitivityText(slider.value);

        slider.onValueChanged.AddListener(
            OnSensitivityChanged
        );
    }

    private void OnSensitivityChanged(float value)
    {
        GameSettings.MouseSensitivity = value;
        UpdateSensitivityText(value);
    }

    private void UpdateSensitivityText(float value)
    {
        if (sensitivityText != null)
        {
            sensitivityText.text = value.ToString("0.0");
        }
    }

    private void OnDestroy()
    {
        if (slider != null)
        {
            slider.onValueChanged.RemoveListener(
                OnSensitivityChanged
            );
        }
    }
}
