using UnityEngine;

public class GuardController : MonoBehaviour
{
    [Header("Настройки движения")]
    public float followSpeed = 15f;
    public float catchDistance = 1.5f;
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
    private int strikes = 0;

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.transform;
        guardRenderer = GetComponent<Renderer>();

        currentDistance = normalDistance;
    }

    void Update()
    {
        if (!isChasing || player == null) return;

        // 1. Логика взятки
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

        // 2. Постепенно восстанавливаем дистанцию, если игрок бежит чисто
        if (currentDistance < normalDistance && strikes < maxStrikes)
        {
            currentDistance += recoverSpeed * Time.deltaTime;
            currentDistance = Mathf.Min(currentDistance, normalDistance);
        }

        // 3. Логика, если игрок набрал 3 ошибки
        if (strikes >= maxStrikes)
        {
            // Страж в ярости: он летит прямо на игрока на максимальной скорости
            currentDistance = 0f;
            followSpeed = 30f; // Резко ускоряем его, чтобы он визуально догнал
        }

        // 4. Целевая позиция
        Vector3 targetPosition = new Vector3(
            player.position.x,
            transform.position.y,
            player.position.z - currentDistance
        );

        // 5. Движение к цели
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, followSpeed * Time.deltaTime);

        // 6. Проверка физической поимки
        if (Vector3.Distance(transform.position, player.position) < catchDistance)
        {
            CatchPlayer();
        }
    }

    public void Bribe()
    {
        isBribed = true;
        bribeTimer = bribeDuration;

        // При взятке сбрасываем ошибки и ярость
        strikes = 0;
        followSpeed = 15f;
        currentDistance = normalDistance;

        if (guardRenderer != null) guardRenderer.material.color = Color.yellow;
    }

    public void OnPlayerHit()
    {
        strikes++;
        Debug.Log("Ошибок подряд: " + strikes);

        // Страж делает рывок вперед
        currentDistance -= lungeDistance;
        currentDistance = Mathf.Clamp(currentDistance, 0f, normalDistance);
    }

    public void OnPlayerSafe()
    {
        if (strikes > 0)
        {
            strikes = 0;
            followSpeed = 15f; // Возвращаем обычную скорость
        }
    }

    void CatchPlayer()
    {
        if (!isChasing) return;
        isChasing = false;
        if (GameManager.Instance != null) GameManager.Instance.GameOver();
    }
}