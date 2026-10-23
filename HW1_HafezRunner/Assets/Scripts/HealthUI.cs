
using TMPro;
using UnityEngine;

public class HealthUI : MonoBehaviour
{
    [Header("References")]
    public PlayerHealth playerHealth;
    public TMP_Text healthText;

    private int lastHealth = -1;
    private int lastMaxHealth = -1;

    void Start()
    {
        RefreshHealthUI();
    }

    void Update()
    {
        if (playerHealth == null || healthText == null)
            return;

        if (lastHealth != playerHealth.CurrentHealth ||
            lastMaxHealth != playerHealth.MaxHealth)
        {
            RefreshHealthUI();
        }
    }

    void RefreshHealthUI()
    {
        if (playerHealth == null || healthText == null)
            return;

        int current = playerHealth.CurrentHealth;
        int maximum = playerHealth.MaxHealth;

        string hearts = "";

        for (int i = 0; i < maximum; i++)
        {
            if (i < current)
            {
                hearts += "<color=#FF3B5C>♥</color> ";
            }
            else
            {
                hearts += "<color=#A0A0A0>♡</color> ";
            }
        }

        healthText.text = hearts.TrimEnd();

        lastHealth = current;
        lastMaxHealth = maximum;
    }
}
