
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Jump Settings")]
    public float jumpForce = 5.5f;

    [Header("Jump Feel")]
    [Range(1f, 4f)]
    public float fallGravityMultiplier = 2.5f;

    [Range(0.5f, 2f)]
    public float riseGravityMultiplier = 1f;

    [Header("Speed Settings")]
    public GameSpeedManager speedManager;

    [Header("Sound Settings")]
    public AudioSource audioSource;
    public AudioClip jumpSound;

    private Rigidbody2D rb;
    private Animator animator;
    private bool isGrounded;
    private float baseGravityScale;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            baseGravityScale = rb.gravityScale;
        }
        else
        {
            Debug.LogError(
                "PlayerController: Rigidbody2D not found!"
            );
        }

        animator = GetComponent<Animator>();

        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }

        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }

    void Update()
    {
        if (Time.timeScale == 0f)
            return;

        UpdateRunAnimationSpeed();

        if (Keyboard.current == null)
            return;

        if (Keyboard.current.spaceKey.wasPressedThisFrame
            && isGrounded)
        {
            Jump();
        }
    }

    void FixedUpdate()
    {
        if (rb == null || isGrounded)
            return;

        if (rb.linearVelocity.y < 0f)
        {
            rb.gravityScale =
                baseGravityScale * fallGravityMultiplier;
        }
        else
        {
            rb.gravityScale =
                baseGravityScale * riseGravityMultiplier;
        }
    }

    void Jump()
    {
        if (rb == null)
            return;

        isGrounded = false;

        rb.gravityScale = baseGravityScale;

        if (animator != null)
        {
            animator.SetBool("IsJumping", true);
        }

        rb.AddForce(
            Vector2.up * jumpForce,
            ForceMode2D.Impulse
        );

        if (audioSource != null && jumpSound != null)
        {
            audioSource.PlayOneShot(jumpSound);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground")
            && rb != null
            && rb.linearVelocity.y <= 0.05f)
        {
            Land();
        }
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground")
            && rb != null
            && rb.linearVelocity.y <= 0.05f)
        {
            Land();
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }

    void Land()
    {
        isGrounded = true;

        if (rb != null)
        {
            rb.gravityScale = baseGravityScale;
        }

        if (animator != null)
        {
            animator.SetBool("IsJumping", false);
        }
    }

    void UpdateRunAnimationSpeed()
    {
        if (animator == null || speedManager == null)
            return;

        bool shouldRunFast =
            isGrounded && speedManager.IsDashing;

        animator.speed = shouldRunFast
            ? speedManager.dashMultiplier
            : 1f;
    }
}
