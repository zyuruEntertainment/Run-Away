using TMPro;
using UnityEngine;

public class StaminaDisplay : MonoBehaviour
{
    public PlayerSprint playerSprint;
    public TMP_Text staminaText;

    void Update()
    {
        if (playerSprint == null || staminaText == null) return;

        staminaText.text = Mathf.CeilToInt(
            playerSprint.CurrentStamina
        ).ToString();
    }
}