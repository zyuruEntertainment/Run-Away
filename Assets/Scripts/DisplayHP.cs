using TMPro;
using UnityEngine;

public class HPDisplay : MonoBehaviour
{
    public PlayerHealth playerHealth;
    public TMP_Text hpText;

    void Update()
    {
        if (playerHealth == null || hpText == null) return;

        hpText.text = playerHealth.currentHP.ToString();
    }
}
