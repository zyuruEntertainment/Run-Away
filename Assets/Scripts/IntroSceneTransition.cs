
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroSceneTransition : MonoBehaviour
{
    [Header("Fade Settings")]
    public CanvasGroup fadeCanvas;
    public float fadeInDuration = 1f;
    public float waitDuration = 1f;
    public float fadeOutDuration = 1f;

    [Header("Scene Settings")]
    public string loadingSceneName = "loading";
    public string targetSceneName = "MainMenu";

    private void Awake()
    {
        Time.timeScale = 1f;

        if (fadeCanvas != null)
        {
            fadeCanvas.alpha = 0f;
            fadeCanvas.interactable = false;
            fadeCanvas.blocksRaycasts = false;
        }
    }

    private void Start()
    {
        StartCoroutine(PlayIntro());
    }

    private IEnumerator PlayIntro()
    {
        if (fadeCanvas != null)
        {
            fadeCanvas.gameObject.SetActive(true);
            fadeCanvas.alpha = 0f;

            yield return Fade(0f, 1f, fadeInDuration);

            yield return new WaitForSecondsRealtime(waitDuration);

            yield return Fade(1f, 0f, fadeOutDuration);
        }

        SceneLoader.TargetSceneName = targetSceneName;

        SceneManager.LoadScene(loadingSceneName);
    }

    private IEnumerator Fade(float startAlpha, float endAlpha, float duration)
    {
        fadeCanvas.alpha = startAlpha;

        if (duration <= 0f)
        {
            fadeCanvas.alpha = endAlpha;
            yield break;
        }

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;

            float progress = Mathf.Clamp01(elapsed / duration);

            fadeCanvas.alpha = Mathf.Lerp(
                startAlpha,
                endAlpha,
                progress
            );

            yield return null;
        }

        fadeCanvas.alpha = endAlpha;
    }
}
