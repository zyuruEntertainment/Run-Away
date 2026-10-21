using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
{
    public GameObject pauseCanvas;
    public List<GameObject> canvasesToDisable = new List<GameObject>();
    public Button pauseButton;
    public Button resumeButton;

    private bool isPaused;
    private PlayerHealth playerHealth;

    private void Start()
    {
        Time.timeScale = 1f;

        playerHealth = FindAnyObjectByType<PlayerHealth>();

        if (pauseCanvas != null)
            pauseCanvas.SetActive(false);

        if (pauseButton != null)
            pauseButton.onClick.AddListener(PauseGame);

        if (resumeButton != null)
            resumeButton.onClick.AddListener(ResumeGame);
    }

    private void Update()
    {
        if (playerHealth != null && playerHealth.IsDead)
            return;

        if (Keyboard.current != null &&
            Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    public void PauseGame()
    {
        if (isPaused)
            return;

        if (playerHealth != null && playerHealth.IsDead)
            return;

        isPaused = true;
        Time.timeScale = 0f;

        if (pauseCanvas != null)
            pauseCanvas.SetActive(true);

        foreach (GameObject canvas in canvasesToDisable)
        {
            if (canvas != null)
                canvas.SetActive(false);
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ResumeGame()
    {
        if (!isPaused)
            return;

        if (playerHealth != null && playerHealth.IsDead)
            return;

        isPaused = false;
        Time.timeScale = 1f;

        if (pauseCanvas != null)
            pauseCanvas.SetActive(false);

        foreach (GameObject canvas in canvasesToDisable)
        {
            if (canvas != null)
                canvas.SetActive(true);
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnDestroy()
    {
        Time.timeScale = 1f;

        if (pauseButton != null)
            pauseButton.onClick.RemoveListener(PauseGame);

        if (resumeButton != null)
            resumeButton.onClick.RemoveListener(ResumeGame);
    }
}