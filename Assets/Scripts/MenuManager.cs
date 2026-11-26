using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [Header("Панели")]
    public GameObject startPanel;
    public GameObject pausePanel;

    [Header("Ссылки")]
    public PlayerController player;

    void Start()
    {
        // При запуске игры ставим паузу и показываем стартовое меню
        Time.timeScale = 0f;

        if (startPanel != null) startPanel.SetActive(true);
        if (pausePanel != null) pausePanel.SetActive(false);
    }

    // Этот метод должен появиться в списке On Click()
    public void StartGame()
    {
        Debug.Log("Кнопка нажата! Запускаем игру.");
        if (startPanel != null) startPanel.SetActive(false);
        Time.timeScale = 1f;
    }

    // Методы для паузы (пригодятся позже)
    public void PauseGame()
    {
        if (pausePanel != null) pausePanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        if (pausePanel != null) pausePanel.SetActive(false);
        Time.timeScale = 1f;
    }
}