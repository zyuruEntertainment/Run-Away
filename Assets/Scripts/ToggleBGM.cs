
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class ToggleBGM : MonoBehaviour
{
    public AudioSource audioSource;

    private Toggle bgmToggle;

    private void Awake()
    {
        bgmToggle = GetComponent<Toggle>();

        bgmToggle.onValueChanged.AddListener(SetBGMVolume);

        SetBGMVolume(bgmToggle.isOn);
    }

    private void OnDestroy()
    {
        if (bgmToggle != null)
            bgmToggle.onValueChanged.RemoveListener(SetBGMVolume);
    }

    private void SetBGMVolume(bool isOn)
    {
        if (audioSource == null)
            return;

        audioSource.volume = isOn ? 1f : 0f;
    }
}
