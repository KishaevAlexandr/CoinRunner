using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("UI Ссылки")]
    public GameObject gameOverPanel;
    public TMP_Text finalCoinsText;
    public TMP_Text scoreText;
    public TMP_Text timerText;

    [Header("Игровые данные")]
    public int totalCoins = 0;
    public int score = 0;
    public float gameTime = 0f;
    public float scoreMultiplier = 10f;

    private bool isGameOver = false;
    private float scoreAccumulator = 0f; // Эта переменная теперь объявлена

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Update()
    {
        // Принудительно обновляем текст каждый кадр (для надежности)
        if (scoreText != null)
        {
            scoreText.text = "Очки: " + score;
        }

        // Основная логика (только если игра идет)
        if (!isGameOver && Time.timeScale > 0)
        {
            // Таймер
            gameTime += Time.deltaTime;
            if (timerText != null)
            {
                int minutes = Mathf.FloorToInt(gameTime / 60f);
                int seconds = Mathf.FloorToInt(gameTime % 60f);
                timerText.text = $"{minutes:00}:{seconds:00}";
            }

            // Начисление очков за время
            scoreAccumulator += scoreMultiplier * Time.deltaTime;
            score = Mathf.FloorToInt(scoreAccumulator);
        }
    }

    public void AddCoin(int amount)
    {
        if (isGameOver) return;
        totalCoins += amount;
    }

    public bool SpendCoins(int amount)
    {
        if (totalCoins >= amount)
        {
            totalCoins -= amount;
            return true;
        }
        return false;
    }

    public void GameOver()
    {
        if (isGameOver) return;
        isGameOver = true;

        if (gameOverPanel != null) gameOverPanel.SetActive(true);
        if (finalCoinsText != null) finalCoinsText.text = $"Монет собрано: {totalCoins}";

        Time.timeScale = 0f;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}