using UnityEngine;
using System.Collections.Generic;

public class LevelGenerator : MonoBehaviour
{
    [Header("Префабы")]
    public GameObject chunkPrefab;
    public GameObject coinPrefab;
    public GameObject obstaclePrefab;

    [Header("Настройки")]
    public int chunkCount = 5; // Сколько кусков держим в памяти
    public float chunkLength = 20f; // Длина одного куска (совпадает с Scale Z)
    public Transform player; // Ссылка на игрока

    private List<GameObject> chunks = new List<GameObject>();
    private float spawnZ = 0f;

    void Start()
    {
        // Создаем начальные куски
        for (int i = 0; i < chunkCount; i++)
        {
            SpawnChunk();
        }
    }

    void Update()
    {
        // Если первый кусок остался далеко позади игрока, переносим его вперед
        if (chunks[0].transform.position.z < player.position.z - chunkLength)
        {
            GameObject oldChunk = chunks[0];
            chunks.RemoveAt(0);

            // Очищаем старый кусок от монет и препятствий
            ClearChunk(oldChunk);

            // Перемещаем его вперед
            oldChunk.transform.position = new Vector3(0, 0, spawnZ);

            // Заново наполняем
            PopulateChunk(oldChunk);

            // Добавляем в конец списка
            chunks.Add(oldChunk);
            spawnZ += chunkLength;
        }
    }

    void SpawnChunk()
    {
        GameObject newChunk = Instantiate(chunkPrefab, new Vector3(0, 0, spawnZ), Quaternion.identity);
        PopulateChunk(newChunk);
        chunks.Add(newChunk);
        spawnZ += chunkLength;
    }

    void PopulateChunk(GameObject chunk)
    {
        // Рандомим, что будет на этом куске
        // Например, 3 ряда монет и 1 препятствие

        // 1. Спавним монеты
        int coinRow = Random.Range(0, 3); // 0, 1 или 2 полоса
        for (int i = 0; i < 5; i++)
        {
            Vector3 pos = new Vector3((coinRow - 1) * 3f, 1f, i * 2f);
            GameObject coin = Instantiate(coinPrefab, chunk.transform);
            coin.transform.localPosition = pos;
        }

        // 2. Спавним препятствие на другой полосе
        int obstacleRow = Random.Range(0, 3);
        if (obstacleRow == coinRow) obstacleRow = (obstacleRow + 1) % 3; // Чтобы не спавнить на монетах

        Vector3 obsPos = new Vector3((obstacleRow - 1) * 3f, 0.75f, Random.Range(5f, 15f));
        GameObject obs = Instantiate(obstaclePrefab, chunk.transform);
        obs.transform.localPosition = obsPos;
    }

    void ClearChunk(GameObject chunk)
    {
        // Удаляем всех детей (монеты и препятствия), кроме пола
        foreach (Transform child in chunk.transform)
        {
            if (child.name != "Floor")
            {
                Destroy(child.gameObject);
            }
        }
    }
}