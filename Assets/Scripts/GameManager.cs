using UnityEngine;
using UnityEngine.SceneManagement; // Понадобится для перезапуска

public class GameManager : MonoBehaviour
{
    public static GameManager Instance; // Синглтон (чтобы обращаться из любого скрипта)

    public int totalCoins = 0;

    void Awake()
    {
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
        totalCoins += amount;
        Debug.Log("Монет собрано: " + totalCoins);

        // Здесь позже мы будем обновлять UI (счетчик на экране)
    }

    public bool SpendCoins(int amount)
    {
        if (totalCoins >= amount)
        {
            totalCoins -= amount;
            Debug.Log("Потрачено монет. Остаток: " + totalCoins);
            return true; // Успешно потратили
        }
        return false; // Не хватает монет
    }
}