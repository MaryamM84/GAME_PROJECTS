
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject mainMenuPanel;
    public GameObject gameOverPanel;
    public GameObject pausePanel;

    [Header("Gameplay References")]
    public PlayerController playerController;

    private bool gameStarted = false;
    private bool gameEnded = false;
    private bool isPaused = false;

    private void Awake()
    {
        Time.timeScale = 1f;

        if (playerController == null)
        {
            playerController =
                FindFirstObjectByType<PlayerController>();
        }
    }

    private void Start()
    {
        gameStarted = false;
        gameEnded = false;
        isPaused = false;

        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(true);

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        if (pausePanel != null)
            pausePanel.SetActive(false);

        if (playerController != null)
        {
            playerController.enabled = false;
        }
        else
        {
            Debug.LogError(
                "GameManager: PlayerController not found."
            );
        }

        Time.timeScale = 0f;
    }

    private void Update()
    {
        if (Keyboard.current == null)
            return;

        if (!gameStarted || gameEnded)
            return;

        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void StartGame()
    {
        if (gameStarted || gameEnded)
            return;

        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.ResetCurrentScore();
        }

        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(false);

        if (pausePanel != null)
            pausePanel.SetActive(false);

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        if (playerController != null)
        {
            playerController.enabled = true;
        }

        isPaused = false;
        gameStarted = true;

        Time.timeScale = 1f;

        Debug.Log("GAME STARTED!");
    }

    public void PauseGame()
    {
        if (!gameStarted || gameEnded || isPaused)
            return;

        isPaused = true;

        if (pausePanel != null)
        {
            pausePanel.SetActive(true);
        }

        Time.timeScale = 0f;

        Debug.Log("GAME PAUSED!");
    }

    public void ResumeGame()
    {
        if (!isPaused || gameEnded)
            return;

        isPaused = false;

        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }

        Time.timeScale = 1f;

        Debug.Log("GAME RESUMED!");
    }

    public void GameOver()
    {
        if (gameEnded)
            return;

        gameEnded = true;
        isPaused = false;

        if (playerController != null)
        {
            playerController.enabled = false;
        }

        if (pausePanel != null)
            pausePanel.SetActive(false);

        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.SaveHighScore();
        }

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        Time.timeScale = 0f;

        Debug.Log("GAME OVER!");
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex
        );
    }
}
