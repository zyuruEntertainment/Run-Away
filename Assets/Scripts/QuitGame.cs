
using UnityEngine;

public class QuitGame : MonoBehaviour
{
    public void Quit()
    {
#if UNITY_EDITOR
        // Berhenti saat dijalankan di Unity Editor
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // Keluar dari game saat sudah di-build
        Application.Quit();
#endif
    }
}
