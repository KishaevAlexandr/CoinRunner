using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [Header("Панели")]
    public GameObject startPanel;
    public GameObject pausePanel;

    [Header("Кнопки")]
    public GameObject pauseButton; // Ссылка на нашу кнопку паузы

    void Start()
    {
        Time.timeScale = 0f;
        if (startPanel != null) startPanel.SetActive(true);
        if (pausePanel != null) pausePanel.SetActive(false);

        // Прячем кнопку паузы при запуске игры
        if (pauseButton != null) pauseButton.SetActive(false);
    }

    public void StartGame()
    {
        if (startPanel != null) startPanel.SetActive(false);

        // Показываем кнопку паузы, когда игра началась
        if (pauseButton != null) pauseButton.SetActive(true);

        Time.timeScale = 1f;
    }

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