using UnityEngine;

public class GuardController : MonoBehaviour
{
    [Header("Настройки движения")]
    public float followSpeed = 15f;
    public float catchDistance = 2f; // Немного увеличили, чтобы поимка срабатывала надежнее
    public float normalDistance = 4f;

    [Header("Настройки наказания")]
    public int maxStrikes = 3;
    public float lungeDistance = 2f;
    public float recoverSpeed = 1.5f;

    [Header("Настройки взятки")]
    public float bribeDuration = 3f;

    private Transform player;
    private bool isChasing = true;

    private bool isBribed = false;
    private float bribeTimer = 0f;
    private Renderer guardRenderer;

    private float currentDistance;
    public int strikes = 0;

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.transform;
        guardRenderer = GetComponent<Renderer>();

        currentDistance = normalDistance;
    }

    void Update()
    {
        if (Time.timeScale == 0f) return;

        if (!isChasing || player == null) return;

        // Логика взятки
        if (isBribed)
        {
            bribeTimer -= Time.deltaTime;
            if (bribeTimer <= 0)
            {
                isBribed = false;
                if (guardRenderer != null) guardRenderer.material.color = Color.red;
            }
            return;
        }

        // --- ЛОГИКА ПОГОНИ ---

        if (strikes < maxStrikes)
        {
            // Обычный режим: держим дистанцию
            if (currentDistance < normalDistance)
            {
                currentDistance += recoverSpeed * Time.deltaTime;
                currentDistance = Mathf.Min(currentDistance, normalDistance);
            }

            Vector3 targetPosition = new Vector3(
                player.position.x,
                transform.position.y,
                player.position.z - currentDistance
            );
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, followSpeed * Time.deltaTime);
        }
        else
        {
            // Режим ЯРОСТИ: страж несется на игрока
            if (guardRenderer != null) guardRenderer.material.color = Color.red;

            // 1. Жестко синхронизируем позицию по X (чтобы он не отставал по полосам)
            float newX = Mathf.Lerp(transform.position.x, player.position.x, 20f * Time.deltaTime);

            // 2. Двигаемся по Z прямо на игрока с высокой скоростью (40)
            float newZ = Mathf.MoveTowards(transform.position.z, player.position.z, 40f * Time.deltaTime);

            // Применяем позицию
            transform.position = new Vector3(newX, transform.position.y, newZ);
        }

        // Проверка поимки (общая)
        if (Vector3.Distance(transform.position, player.position) < catchDistance)
        {
            CatchPlayer();
        }
    }

    public void Bribe()
    {
        isBribed = true;
        bribeTimer = bribeDuration;
        strikes = 0;
        currentDistance = normalDistance;
        if (guardRenderer != null) guardRenderer.material.color = Color.yellow;
    }

    public void OnPlayerHit()
    {
        strikes++;
        currentDistance -= lungeDistance;
        currentDistance = Mathf.Clamp(currentDistance, 0f, normalDistance);
    }

    public void OnPlayerSafe()
    {
        if (strikes > 0)
        {
            strikes = 0;
            if (guardRenderer != null) guardRenderer.material.color = Color.red;
        }
    }

    public void ResetChase()
    {
        isChasing = true;
        strikes = 0;
        currentDistance = normalDistance;
        isBribed = false;
    }

    void CatchPlayer()
    {
        if (!isChasing) return;
        isChasing = false;
        if (GameManager.Instance != null) GameManager.Instance.GameOver();
    }
}