
using UnityEngine;

public class GroundObstacle : Interactable
{
    public override void Interact(
        PlayerController player)
    {
        PlayerHealth health =
            player.GetComponent<PlayerHealth>();

        if (health != null)
        {
            Debug.Log(
                "Ground obstacle hit: "
                + gameObject.name
            );

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
