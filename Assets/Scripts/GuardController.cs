using UnityEngine;

public class GuardController : MonoBehaviour
{
    [Header("Настройки движения")]
    public float speedZ = 9f;
    public float speedX = 15f;
    public float catchDistance = 1.5f;
    public float followDistance = 3f;

    [Header("Настройки взятки")]
    public float bribeDuration = 3f;

    [Header("Настройки внимания")]
    public float attentionLevel = 0f;
    public float attentionIncrease = 25f;
    public float attentionDecay = 5f;

    private float baseSpeedZ; // Запоминаем изначальную скорость
    private Transform player;
    private bool isChasing = true;

    private bool isBribed = false;
    private float bribeTimer = 0f;
    private Renderer guardRenderer;

    void Start()
    {
        baseSpeedZ = speedZ; // Запоминаем базовую скорость
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.transform;
        guardRenderer = GetComponent<Renderer>();
    }

    void Update()
    {
        // 1. Логика внимания (работает всегда, даже если дали взятку)
        if (attentionLevel > 0 && !isBribed)
        {
            attentionLevel -= attentionDecay * Time.deltaTime;
            attentionLevel = Mathf.Clamp(attentionLevel, 0f, 100f);
        }

        // 2. Ускорение при высоком внимании
        if (attentionLevel > 50f)
        {
            speedZ = baseSpeedZ * 1.3f; // Ускоряемся на 30%
        }
        else
        {
            speedZ = baseSpeedZ;
        }

        // 3. Логика взятки
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

        if (!isChasing || player == null) return;

        // 4. Движение
        float targetX = player.position.x;
        float targetZ = player.position.z - followDistance;

        float newX = Mathf.Lerp(transform.position.x, targetX, speedX * Time.deltaTime);
        float newZ = Mathf.MoveTowards(transform.position.z, targetZ, speedZ * Time.deltaTime);

        transform.position = new Vector3(newX, transform.position.y, newZ);

        // 5. Проверка поимки
        if (Vector3.Distance(transform.position, player.position) < catchDistance)
        {
            CatchPlayer();
        }
    }

    // --- НОВЫЙ МЕТОД: Получение взятки ---
    public void Bribe()
    {
        isBribed = true;
        bribeTimer = bribeDuration;
        if (guardRenderer != null) guardRenderer.material.color = Color.yellow;
        Debug.Log("Стражник получил взятку!");
    }

    // --- НОВЫЙ МЕТОД: Реакция на удар игрока ---
    public void OnPlayerHit()
    {
        attentionLevel += attentionIncrease;
        attentionLevel = Mathf.Clamp(attentionLevel, 0f, 100f);
        Debug.Log("Внимание стражи: " + attentionLevel + "%");
    }

    void CatchPlayer()
    {
        Debug.Log("Стражник поймал игрока! Game Over.");
        isChasing = false;
    }
}