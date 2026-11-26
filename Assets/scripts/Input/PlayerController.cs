using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class PlayerController : MonoBehaviour
{
    [Header("Настройки движения")]
    public float forwardSpeed = 10f;
    public float laneDistance = 3f;
    public float laneChangeSpeed = 15f;

    [Header("Настройки прыжка")]
    public float jumpForce = 8f;
    public float gravity = -20f;

    [Header("Настройки подката")]
    public float slideDuration = 0.8f;
    public float slideHeight = 0.5f;

    [Header("Настройки безопасности")]
    public float safeThreshold = 3f;

    [Header("UI Ошибок")]
    public TMP_Text strikesText;

    [Header("Визуализация")]
    public GameObject coinBagPrefab; // Ссылка на префаб мешка с монетами

    private CharacterController controller;
    private Vector3 velocity;
    private int currentLane = 1;
    private bool isSliding = false;
    private float slideTimer = 0f;
    private float originalHeight;
    private Vector3 originalCenter;

    private Vector2 touchStartPos;
    private bool isSwiping = false;

    private float speedMultiplier = 1f;
    private float safeTimer = 0f;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        originalHeight = controller.height;
        originalCenter = controller.center;

        if (strikesText != null) strikesText.text = "Ошибки: 0/3";
    }

    void Update()
    {
        if (Time.timeScale == 0f) return;

        HandleInput();
        MovePlayer();
        HandleSlide();

        safeTimer += Time.deltaTime;
        if (safeTimer >= safeThreshold)
        {
            GuardController guard = FindFirstObjectByType<GuardController>();
            if (guard != null)
            {
                guard.OnPlayerSafe();

                if (strikesText != null) strikesText.text = "Ошибки: 0/3";
            }
            safeTimer = 0f;
        }
    }

    void HandleInput()
    {
        var keyboard = Keyboard.current;
        if (keyboard != null)
        {
            if (keyboard.aKey.wasPressedThisFrame || keyboard.leftArrowKey.wasPressedThisFrame) MoveLeft();
            if (keyboard.dKey.wasPressedThisFrame || keyboard.rightArrowKey.wasPressedThisFrame) MoveRight();
            if (keyboard.spaceKey.wasPressedThisFrame) Jump();
            if (keyboard.sKey.wasPressedThisFrame || keyboard.leftShiftKey.wasPressedThisFrame) StartSlide();
            if (keyboard.eKey.wasPressedThisFrame) Bribe();
        }

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

                if (swipeDelta.magnitude > 50)
                {
                    if (Mathf.Abs(swipeDelta.x) > Mathf.Abs(swipeDelta.y))
                    {
                        if (swipeDelta.x > 0) MoveRight();
                        else MoveLeft();
                    }
                    else
                    {
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
        float moveZ = forwardSpeed * speedMultiplier;
        float targetX = (currentLane - 1) * laneDistance;
        float xMove = Mathf.MoveTowards(transform.position.x, targetX, laneChangeSpeed * Time.deltaTime) - transform.position.x;

        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
        velocity.y += gravity * Time.deltaTime;

        Vector3 finalMove = new Vector3(xMove / Time.deltaTime, velocity.y, moveZ);
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
        if (GameManager.Instance != null && GameManager.Instance.SpendCoins(30))
        {
            GuardController guard = FindFirstObjectByType<GuardController>();
            if (guard != null) guard.Bribe();
        }
        else
        {
            Debug.Log("Недостаточно монет для взятки! Нужно 30.");
        }
    }

    public void TakeHit()
    {
        speedMultiplier = 0.5f;
        CancelInvoke(nameof(RecoverSpeed));
        Invoke(nameof(RecoverSpeed), 0.5f);

        GuardController guard = FindFirstObjectByType<GuardController>();
        if (guard != null)
        {
            guard.OnPlayerHit();

            if (strikesText != null)
            {
                strikesText.text = "Ошибки: " + guard.strikes + "/3";
            }
        }

        safeTimer = 0f;
    }

    void RecoverSpeed()
    {
        speedMultiplier = 1f;
    }
}