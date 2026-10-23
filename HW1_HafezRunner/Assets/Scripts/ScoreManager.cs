
using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    private const string HighScoreKey = "HighScore";

    [Header("Score Settings")]
    public int score = 0;

    [SerializeField] private int highScore = 0;

    [Min(0f)]
    public float pointsPerSecond = 10f;

    [Header("UI References")]
    public TMP_Text scoreText;
    public TMP_Text highScoreText;
    public TMP_Text finalScoreText;

    private float scoreAccumulator;

    public int CurrentScore => score;
    public int HighScore => highScore;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogError(
                "More than one ScoreManager exists in the scene."
            );

            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        score = 0;
        scoreAccumulator = 0f;

        highScore = PlayerPrefs.GetInt(HighScoreKey, 0);

        UpdateScoreUI();

        Debug.Log("Loaded High Score: " + highScore);
    }

    private void Update()
    {
        
        if (Time.timeScale <= 0f)
            return;

        if (pointsPerSecond <= 0f)
            return;

        
        scoreAccumulator += Time.deltaTime * pointsPerSecond;

        int earnedPoints = Mathf.FloorToInt(scoreAccumulator);

        if (earnedPoints > 0)
        {
            scoreAccumulator -= earnedPoints;
            AddScore(earnedPoints);
        }
    }

    public void AddScore(int amount)
    {
        if (amount <= 0)
            return;

        score += amount;

        if (score > highScore)
        {
            highScore = score;

            
            PlayerPrefs.SetInt(HighScoreKey, highScore);
        }

        UpdateScoreUI();
    }

    public void SaveHighScore()
    {
        if (score > highScore)
        {
            highScore = score;
        }

        PlayerPrefs.SetInt(HighScoreKey, highScore);
        PlayerPrefs.Save();

        UpdateScoreUI();

        Debug.Log("High Score Saved: " + highScore);
    }

    public void ResetCurrentScore()
    {
        score = 0;
        scoreAccumulator = 0f;

        UpdateScoreUI();
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score.ToString("D5");
        }

        if (highScoreText != null)
        {
            highScoreText.text =
                "HighScore: " + highScore.ToString("D5");
        }

        if (finalScoreText != null)
        {
            finalScoreText.text =
                "FINAL SCORE: " + score.ToString("D5");
        }
    }

    private void OnApplicationPause(bool paused)
    {
        if (paused)
        {
            SaveHighScore();
        }
    }

    private void OnApplicationQuit()
    {
        SaveHighScore();
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
}
