using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Настройки движения")]
    public float forwardSpeed = 10f;
    public float laneDistance = 3f; // Расстояние между полосами
    public float laneChangeSpeed = 15f;

    [Header("Настройки прыжка")]
    public float jumpForce = 8f;
    public float gravity = -20f;

    [Header("Настройки подката")]
    public float slideDuration = 0.8f;
    public float slideHeight = 0.5f; // Высота коллайдера при подкате

    private CharacterController controller;
    private Vector3 velocity;
    private int currentLane = 1; // 0 - лево, 1 - центр, 2 - право
    private bool isSliding = false;
    private float slideTimer = 0f;
    private float originalHeight;
    private Vector3 originalCenter;

    // Для свайпов
    private Vector2 touchStartPos;
    private bool isSwiping = false;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        originalHeight = controller.height;
        originalCenter = controller.center;
    }

    void Update()
    {
        HandleInput();
        MovePlayer();
        HandleSlide();
    }

    void HandleInput()
    {
        // 1. Управление с клавиатуры (через Input System)
        var keyboard = Keyboard.current;
        if (keyboard != null)
        {
            if (keyboard.aKey.wasPressedThisFrame || keyboard.leftArrowKey.wasPressedThisFrame) MoveLeft();
            if (keyboard.dKey.wasPressedThisFrame || keyboard.rightArrowKey.wasPressedThisFrame) MoveRight();
            if (keyboard.spaceKey.wasPressedThisFrame) Jump();
            if (keyboard.sKey.wasPressedThisFrame || keyboard.leftShiftKey.wasPressedThisFrame) StartSlide();
            if (keyboard.eKey.wasPressedThisFrame) Bribe();
        }

        // 2. Управление свайпами (для мобильных устройств)
        if (Touchscreen.current != null)
        {
            var touch = Touchscreen.current.primaryTouch;
            if (touch.press.wasPressedThisFrame)
            {
                touchStartPos = touch.position.ReadValue();
                isSwiping = true;
            }
            else if (touch.press.wasReleasedThisFrame && isSwiping)
            {
                Vector2 touchEndPos = touch.position.ReadValue();
                Vector2 swipeDelta = touchEndPos - touchStartPos;

                if (swipeDelta.magnitude > 50) // Минимальная длина свайпа
                {
                    if (Mathf.Abs(swipeDelta.x) > Mathf.Abs(swipeDelta.y))
                    {
                        // Свайп влево/вправо
                        if (swipeDelta.x > 0) MoveRight();
                        else MoveLeft();
                    }
                    else
                    {
                        // Свайп вверх/вниз
                        if (swipeDelta.y > 0) Jump();
                        else StartSlide();
                    }
                }
                isSwiping = false;
            }
        }
    }

    void MovePlayer()
    {
        // 1. Движение вперед 
        float moveZ = forwardSpeed;

        // 2. Плавное перемещение по X к нужной полосе
        float targetX = (currentLane - 1) * laneDistance;
        float xMove = Mathf.MoveTowards(transform.position.x, targetX, laneChangeSpeed * Time.deltaTime) - transform.position.x;

        //Гравитация
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
        velocity.y += gravity * Time.deltaTime;

        Vector3 finalMove = new Vector3(xMove / Time.deltaTime, velocity.y, moveZ);

        // Применяем движение
        controller.Move(finalMove * Time.deltaTime);
    }

    void MoveLeft() { if (currentLane > 0) currentLane--; }
    void MoveRight() { if (currentLane < 2) currentLane++; }

    void Jump()
    {
        if (controller.isGrounded && !isSliding)
        {
            velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
        }
    }

    void StartSlide()
    {
        if (!isSliding)
        {
            isSliding = true;
            slideTimer = slideDuration;
            controller.height = slideHeight;
            controller.center = new Vector3(0, slideHeight / 2, 0);
        }
    }

    void HandleSlide()
    {
        if (isSliding)
        {
            slideTimer -= Time.deltaTime;
            if (slideTimer <= 0)
            {
                isSliding = false;
                controller.height = originalHeight;
                controller.center = originalCenter;
            }
        }
    }

    void Bribe()
    {
        // Заглушка для будущей механики взятки
        Debug.Log("Попытка дать взятку! Монет потрачено: 10");
    }
}