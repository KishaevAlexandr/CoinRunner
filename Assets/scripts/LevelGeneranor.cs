using UnityEngine;
using System.Collections.Generic;

public class LevelGenerator : MonoBehaviour
{
    [Header("Префабы")]
    public GameObject chunkPrefab;
    public GameObject coinPrefab;
    public GameObject obstaclePrefab;
    public GameObject highWallPrefab;
    public GameObject lowBarPrefab;

    [Header("Настройки")]
    public int chunkCount = 5;
    public float chunkLength = 20f;
    public Transform player;

    private List<GameObject> chunks = new List<GameObject>();
    private float spawnZ = 0f;

    void Start()
    {
        for (int i = 0; i < chunkCount; i++)
        {
            SpawnChunk();
        }
    }

    void Update()
    {
        // --- ПРОВЕРКА НА ПАУЗУ ---
        if (Time.timeScale == 0f) return;

        if (player == null) return;

        if (chunks[0].transform.position.z < player.position.z - chunkLength)
        {
            GameObject oldChunk = chunks[0];
            chunks.RemoveAt(0);

            ClearChunk(oldChunk);

            oldChunk.transform.position = new Vector3(0, 0, spawnZ);

            PopulateChunk(oldChunk);

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
        int coinRow = Random.Range(0, 3);
        for (int i = 0; i < 5; i++)
        {
            Vector3 pos = new Vector3((coinRow - 1) * 3f, 1f, i * 2f);
            GameObject coin = Instantiate(coinPrefab, chunk.transform);
            coin.transform.localPosition = pos;
        }

        int obstacleType = Random.Range(0, 3);
        int obstacleRow = Random.Range(0, 3);

        if (obstacleRow == coinRow)
        {
            obstacleRow = (obstacleRow + 1) % 3;
        }

        float obstacleZ = Random.Range(5f, 15f);
        Vector3 obsPos = new Vector3((obstacleRow - 1) * 3f, 0, obstacleZ);

        if (obstacleType == 0)
        {
            GameObject spawnedObstacle = Instantiate(obstaclePrefab, chunk.transform);
            spawnedObstacle.transform.localPosition = new Vector3(obsPos.x, 0.75f, obsPos.z);
        }
        else if (obstacleType == 1)
        {
            GameObject spawnedObstacle = Instantiate(highWallPrefab, chunk.transform);
            spawnedObstacle.transform.localPosition = new Vector3(obsPos.x, 0.5f, obsPos.z);
        }
        else
        {
            GameObject spawnedObstacle = Instantiate(lowBarPrefab, chunk.transform);
            spawnedObstacle.transform.localPosition = new Vector3(obsPos.x, 2f, obsPos.z);
        }
    }

    void ClearChunk(GameObject chunk)
    {
        foreach (Transform child in chunk.transform)
        {
            if (child.name != "Floor")
            {
                Destroy(child.gameObject);
            }
        }
    }
}