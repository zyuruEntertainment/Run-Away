using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHP = 100;
    public int currentHP = 100;
    public Camera playerCamera;
    public Transform monster;
    public GameObject gameOverPanel;
    public MonoBehaviour[] playerControls;

    private bool isDead;

    void Start()
    {
        currentHP = maxHP;

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHP = Mathf.Max(0, currentHP - damage);

        if (currentHP <= 0)
            GameOver();
    }

    void GameOver()
    {
        isDead = true;

        foreach (MonoBehaviour control in playerControls)
        {
            if (control != null)
                control.enabled = false;
        }

        if (playerCamera != null && monster != null)
        {
            Vector3 direction =
                monster.position - playerCamera.transform.position;

            if (direction.sqrMagnitude > 0.001f)
            {
                playerCamera.transform.rotation =
                    Quaternion.LookRotation(direction);
            }
        }

        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Time.timeScale = 0f;
    }
}
