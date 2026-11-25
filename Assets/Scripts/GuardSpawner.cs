using UnityEngine;

public class GuardSpawner : MonoBehaviour
{
    [Header("Настройки спавна")]
    public GameObject guardPrefab; // Сюда перетащим префаб
    public float spawnZPosition = -5f; // Где он появится (позади игрока)

    private GameObject currentGuard;

    void Start()
    {
        // Спавним стражника при старте игры
        SpawnGuard();
    }

    public void SpawnGuard()
    {
        if (guardPrefab != null)
        {
            // Создаем стражника в нужной позиции
            currentGuard = Instantiate(guardPrefab, new Vector3(0, 0, spawnZPosition), Quaternion.identity);
            Debug.Log("Стражник заспавнен!");
        }
        else
        {
            Debug.LogError("Префаб стражника не назначен в GuardSpawner!");
        }
    }

    // (Опционально) Метод для перезапуска стражника, пригодится позже
    public void ResetGuard()
    {
        if (currentGuard != null) Destroy(currentGuard);
        SpawnGuard();
    }
}