
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class ObstacleMover : MonoBehaviour
{
    [Header("Movement")]
    public GameSpeedManager speedManager;
    public float speedMultiplier = 1f;
    public float destroyX = -15f;

    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0f;
        rb.constraints =
            RigidbodyConstraints2D.FreezeRotation;
    }

    void FixedUpdate()
    {
        if (speedManager == null)
            return;

        float currentSpeed =
            speedManager.CurrentSpeed
            * speedMultiplier;

        rb.MovePosition(
            rb.position
            + Vector2.left
            * currentSpeed
            * Time.fixedDeltaTime
        );

        if (transform.position.x < destroyX)
        {
            Destroy(gameObject);
        }
    }
}
