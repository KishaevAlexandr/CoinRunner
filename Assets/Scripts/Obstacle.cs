using UnityEngine;

public class Obstacle : MonoBehaviour
{
    private bool hasHit = false; // Защита от двойного срабатывания

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasHit)
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                player.TakeHit();
                hasHit = true; // Больше не бьем этого игрока
            }
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.CompareTag("Player") && !hasHit)
        {
            PlayerController player = hit.gameObject.GetComponent<PlayerController>();
            if (player != null)
            {
                player.TakeHit();
                hasHit = true;
            }
        }
    }

    // Сброс флага, если объект переиспользуется (например, при генерации)
    private void OnEnable()
    {
        hasHit = false;
    }
}