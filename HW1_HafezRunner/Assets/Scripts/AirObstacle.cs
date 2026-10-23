
using UnityEngine;

public class AirObstacle : Interactable
{
    public override void Interact(
        PlayerController player)
    {
        PlayerHealth health =
            player.GetComponent<PlayerHealth>();

        if (health != null)
        {
            Debug.Log("Air obstacle hit!");

            health.TakeDamage(1);
        }
        else
        {
            Debug.LogWarning(
                "PlayerHealth is missing on Player!"
            );
        }
    }
}
