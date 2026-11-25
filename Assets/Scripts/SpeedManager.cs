using UnityEngine;

public class SpeedManager : MonoBehaviour
{
    public PlayerController player;

    public float speedIncreaseInterval = 10f;
    public float speedIncreaseAmount = 0.5f;
    public float maxSpeed = 20f;

    private float timer = 0f;

    void Start()
    {
        if (player == null) player = FindFirstObjectByType<PlayerController>();
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= speedIncreaseInterval)
        {
            IncreaseSpeed();
            timer = 0f;
        }
    }

    void IncreaseSpeed()
    {
        // Ускоряем ТОЛЬКО игрока
        if (player != null && player.forwardSpeed < maxSpeed)
        {
            player.forwardSpeed += speedIncreaseAmount;
            Debug.Log("Скорость мира увеличена: " + player.forwardSpeed);
        }
    }
}