using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("UI Ссылки")]
    public GameObject gameOverPanel;
    public GameObject continuePanel; // Панель "Продолжить за монеты"
    public TMP_Text continueCostText; // Текст "Продолжить за X монет"
    public TMP_Text finalCoinsText;
    public TMP_Text scoreText;
    public TMP_Text timerText;

    [Header("Игровые данные")]
    public int totalCoins = 0;
    public int score = 0;
    public float gameTime = 0f;
    public float scoreMultiplier = 10f; // 10 очков за секунду
    public int continueCost = 20; // Цена спасения

    private bool isGameOver = false;
    private float scoreAccumulator = 0f;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Update()
    {
        // Принудительно обновляем текст каждый кадр
        if (scoreText != null)
        {
            scoreText.text = "Очки: " + score;
        }

        // Обновляем таймер и очки только если игра идет
        if (!isGameOver && Time.timeScale > 0)
        {
            // 1. Таймер
            gameTime += Time.deltaTime;
            if (timerText != null)
            {
                int minutes = Mathf.FloorToInt(gameTime / 60f);
                int seconds = Mathf.FloorToInt(gameTime % 60f);
                timerText.text = $"{minutes:00}:{seconds:00}";
            }

            // 2. Очки за расстояние
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

    // --- МЕТОД ОКОНЧАНИЯ ИГРЫ ---
    public void GameOver()
    {
        if (isGameOver) return;
        isGameOver = true;

        Debug.Log("Игрок пойман!");
        
        // Ставим игру на паузу
        Time.timeScale = 0f; 

        // Если монет хватает на продолжение — показываем окно "Продолжить"
        if (totalCoins >= continueCost && continuePanel != null)
        {
            if (continueCostText != null) 
                continueCostText.text = $"Продолжить за {continueCost} монет?";
            
            continuePanel.SetActive(true);
        }
        else
        {
            // Иначе показываем финальный Game Over
            if (gameOverPanel != null) gameOverPanel.SetActive(true);
            if (finalCoinsText != null) finalCoinsText.text = $"Монет собрано: {totalCoins}";
        }
    }

    // --- МЕТОД ПРОДОЛЖЕНИЯ ИГРЫ (вызывается кнопкой) ---
    public void ContinueGame()
    {
        // Пытаемся списать монеты
        if (SpendCoins(continueCost))
        {
            isGameOver = false;
            Time.timeScale = 1f;
            
            if (continuePanel != null) continuePanel.SetActive(false);

            // Отодвигаем стражника назад, чтобы дать игроку шанс убежать
            GuardController guard = FindFirstObjectByType<GuardController>();
            if (guard != null)
            {
                // Сдвигаем стражника на 15 метров назад от игрока
                Vector3 newPos = guard.transform.position;
                newPos.z = guard.transform.position.z - 15f;
                guard.transform.position = newPos;

                // Сбрасываем погоню (чтобы он не был в ярости)
                guard.ResetChase(); 
            }
        }
    }

    public void RestartGame()
    {
        Time.timeScale = 1f; 
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); 
    }
}