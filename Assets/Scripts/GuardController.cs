using UnityEngine;

public class GuardController : MonoBehaviour
{
    [Header("Настройки движения")]
    public float speedZ = 9f; // Скорость догона (чуть медленнее игрока)
    public float speedX = 15f; // Скорость перестроения (сделал выше, чтобы успевал)
    public float catchDistance = 1.5f;
    public float followDistance = 3f; // Дистанция позади игрока

    [Header("Настройки взятки")]
    public float bribeDuration = 3f;

    private Transform player;
    private bool isChasing = true;

    private bool isBribed = false;
    private float bribeTimer = 0f;
    private Renderer guardRenderer;

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.transform;
        guardRenderer = GetComponent<Renderer>();
    }

    void Update()
    {
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

        // 1. Вычисляем целевую позицию: мы хотим быть на той же линии X, что и игрок, но позади по Z
        float targetX = player.position.x;
        float targetZ = player.position.z - followDistance;

        // 2. Плавно двигаем стражника по X (в сторону игрока)
        // Мы используем Lerp для плавности, но с высокой скоростью
        float newX = Mathf.Lerp(transform.position.x, targetX, speedX * Time.deltaTime);

        // 3. Плавно двигаем по Z (догоняем)
        float newZ = Mathf.MoveTowards(transform.position.z, targetZ, speedZ * Time.deltaTime);

        // Применяем новую позицию
        transform.position = new Vector3(newX, transform.position.y, newZ);

        // 4. Проверка поимки
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

        Debug.Log("Стражник получил взятку!");
    }

    void CatchPlayer()
    {
        Debug.Log("Стражник поймал игрока! Game Over.");
        isChasing = false;
    }
}