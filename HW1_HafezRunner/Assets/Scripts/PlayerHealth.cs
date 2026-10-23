
using System.Collections;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 3;

    [Header("Damage Settings")]
    public float invincibilityTime = 1f;

    [Header("Animation Settings")]
    public Animator playerAnimator;
    public float deathAnimationDelay = 0.5f;

    [Header("Death Sound")]
    public AudioClip deathSound;

    [Header("Hit Sound")]
    public AudioClip hitSound;

    private int currentHealth;
    private float nextDamageTime;
    private bool isDead;

    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;

    void Awake()
    {
        if (playerAnimator == null)
        {
            playerAnimator =
                GetComponentInChildren<Animator>();
        }
    }

    void Start()
    {
        Time.timeScale = 1f;

        currentHealth = maxHealth;
        nextDamageTime = 0f;
        isDead = false;

        Debug.Log(
            "Game Started! Health: " + currentHealth
        );
    }


    public void TakeDamage(int damage)
    {
        if (isDead || Time.time < nextDamageTime)
            return;

        if (damage <= 0)
            return;

        currentHealth = Mathf.Max(
            currentHealth - damage,
            0
        );

        nextDamageTime =
            Time.time + invincibilityTime;

        Debug.Log(
            "Player Health: " + currentHealth
        );

        if (currentHealth <= 0)
        {
            Die();
            return;
        }

        AudioSource source = GetComponent<AudioSource>();

        if (source == null)
        {
            source = GetComponentInChildren<AudioSource>();
        }

        if (source != null && hitSound != null)
        {
            source.PlayOneShot(hitSound);
        }

        if (playerAnimator != null)
        {
            playerAnimator.SetTrigger("Hit");
        }
        else
        {
            Debug.LogWarning(
                "Player Animator was not found!"
            );
        }
    }


    void Die()
    {
        if (isDead)
            return;

        isDead = true;

        AudioSource source = GetComponent<AudioSource>();

        if (source == null)
        {
            source = GetComponentInChildren<AudioSource>();
        }

        if (source != null && deathSound != null)
        {
            source.PlayOneShot(deathSound);
        }

        PlayerController controller =
            GetComponent<PlayerController>();

        if (controller != null)
        {
            controller.enabled = false;
        }

        if (playerAnimator == null)
        {
            playerAnimator =
                GetComponentInChildren<Animator>();
        }

        if (playerAnimator != null)
        {
            playerAnimator.SetBool("IsDead", true);
        }
        else
        {
            Debug.LogWarning(
                "Player Animator was not found!"
            );
        }

        StartCoroutine(StopGameAfterDeath());
    }


    IEnumerator StopGameAfterDeath()
    {
        yield return new WaitForSecondsRealtime(
            deathAnimationDelay
        );

        Debug.Log("GAME OVER!");

        GameManager gameManager =
            FindAnyObjectByType<GameManager>();

        if (gameManager != null)
        {
            gameManager.GameOver();
        }
        else
        {
            Debug.LogWarning("GameManager not found!");
            Time.timeScale = 0f;
        }
    }

}
