
using UnityEngine;

public class ScoreItem : Interactable
{
    public int coinValue = 10;

    private bool collected = false;

    public override void Interact(PlayerController player)
    {
        if (collected)
            return;

        if (ScoreManager.Instance == null)
        {
            Debug.LogError(
                "ScoreItem: No ScoreManager found in the scene."
            );

            return;
        }

        collected = true;

        ScoreManager.Instance.AddScore(coinValue);

        Debug.Log("Coin collected: +" + coinValue);

        Destroy(gameObject);
    }
}
