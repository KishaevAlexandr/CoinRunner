using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; // Обязательно для работы с текстом TextMeshPro

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("UI Настройки")]
    public GameObject gameOverPanel;
    public TMP_Text finalCoinsText; // Ссылка на текст с монетами

    public int totalCoins = 0;
    private bool isGameOver = false;

    void Awake()
    {
        // Настраиваем синглтон
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddCoin(int amount)
    {
        if (isGameOver) return; // Если игра кончена, монеты не собираются
        totalCoins += amount;

        // Здесь можно добавить обновление UI в реальном времени (счетчик в углу)
        // Debug.Log("Монет собрано: " + totalCoins);
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

    // --- Метод окончания игры ---
    public void GameOver()
    {
        if (isGameOver) return; // Защита от двойного вызова
        isGameOver = true;

        Debug.Log("Игра окончена! Игрок пойман.");

        // 1. Показываем панель Game Over
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        // 2. Обновляем текст с монетами
        if (finalCoinsText != null)
        {
            // Используем интерполяцию строк для красивого вывода
            finalCoinsText.text = $"Монет собрано: {totalCoins}";
        }

        // 3. Замораживаем игру (останавливаем время и физику)
        Time.timeScale = 0f;
    }

    // --- Метод перезапуска ---
    public void RestartGame()
    {
        Time.timeScale = 1f; // ОБЯЗАТЕЛЬНО возвращаем время перед перезагрузкой!

        // Перезагружаем текущую сцену
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}