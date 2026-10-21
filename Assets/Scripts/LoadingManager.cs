using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class LoadingManager : MonoBehaviour
{
    public TMP_Text loadingText;

    private IEnumerator Start()
    {
        Time.timeScale = 1f;

        string targetScene = SceneLoader.TargetSceneName;

        if (string.IsNullOrEmpty(targetScene))
        {
            Debug.LogError("target scene belum ditentukan!");
            yield break;
        }

        AsyncOperation operation =
            SceneManager.LoadSceneAsync(targetScene);

        operation.allowSceneActivation = false;

        while (operation.progress < 0.9f)
        {
            float progress = Mathf.Clamp01(
                operation.progress / 0.9f
            );

            UpdateLoadingUI(progress);

            yield return null;
        }

        UpdateLoadingUI(1f);

        yield return new WaitForSecondsRealtime(0.2f);

        operation.allowSceneActivation = true;
    }

    private void UpdateLoadingUI(float progress)
    {
        int percentage = Mathf.Clamp(
            Mathf.RoundToInt(progress * 100f),
            1,
            100
        );

        if (loadingText != null)
            loadingText.text = percentage.ToString();
    }
}