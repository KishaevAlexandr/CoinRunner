using UnityEngine;

public class Coin : MonoBehaviour
{
    // Вращение монеты для красоты (опционально)
    public float rotationSpeed = 180f;

    void Update()
    {
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        // Если в монету врезался игрок
        if (other.CompareTag("Player"))
        {
            // Добавляем монету в GameManager
            if (GameManager.Instance != null)
            {
                GameManager.Instance.AddCoin(1);
            }

            // Уничтожаем монету на сцене
            Destroy(gameObject);
        }
    }
}