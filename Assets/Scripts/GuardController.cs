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

    private float baseSpeedZ;
    private Transform player;
    private bool isChasing = true;

    private bool isBribed = false;
    private float bribeTimer = 0f;
    private Renderer guardRenderer;

    void Start()
    {
        baseSpeedZ = speedZ;
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.transform;
        guardRenderer = GetComponent<Renderer>();
    }

    void Update()
    {
        // 1. Логика внимания
        if (attentionLevel > 0 && !isBribed)
        {
            attentionLevel -= attentionDecay * Time.deltaTime;
            attentionLevel = Mathf.Clamp(attentionLevel, 0f, 100f);
        }

        // 2. Ускорение и сокращение дистанции при высоком внимании
        if (attentionLevel > 50f)
        {
            speedZ = baseSpeedZ * 1.5f; // Ускоряемся на 50% (чтобы точно догнать)
            followDistance = 1f; // Подбираемся ближе
        }
        else
        {
            speedZ = baseSpeedZ;
            followDistance = 3f; // Возвращаем дистанцию
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

    public void Bribe()
    {
        isBribed = true;
        bribeTimer = bribeDuration;
        if (guardRenderer != null) guardRenderer.material.color = Color.yellow;
    }

    public void OnPlayerHit()
    {
        attentionLevel += attentionIncrease;
        attentionLevel = Mathf.Clamp(attentionLevel, 0f, 100f);
    }

    void CatchPlayer()
    {
        if (!isChasing) return;

        isChasing = false;
        if (GameManager.Instance != null)
        {
            GameManager.Instance.GameOver();
        }
    }
}