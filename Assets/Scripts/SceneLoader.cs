using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public string targetSceneName;
    public static string TargetSceneName;

    public void LoadScene()
    {
        if (string.IsNullOrEmpty(targetSceneName))
        {
            Debug.LogError("target scene belum diisi!");
            return;
        }

        TargetSceneName = targetSceneName;
        SceneManager.LoadScene("loading");
    }
}