
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    protected virtual void OnTriggerEnter2D(
        Collider2D other)
    {
        PlayerController player =
            other.GetComponent<PlayerController>();

        if (player != null)
        {
            Interact(player);
        }
    }

    public abstract void Interact(
        PlayerController player);
}
