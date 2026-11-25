using UnityEngine;

public class Obstacle : MonoBehaviour
{
    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        // Этот метод работает специально для Character Controller
        if (hit.gameObject.CompareTag("Player"))
        {
            // Пока просто выводим в консоль
            Debug.Log("Игрок врезался в препятствие!");

            // В будущем здесь мы будем уменьшать выносливость или 
            // повышать уровень внимания стражи
        }
    }
}