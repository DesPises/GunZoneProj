using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    [SerializeField] private GameObject settings;
    [SerializeField] private GameObject pauseMenu;
    private bool canPause;
    public static bool isPaused;


    private void Start()
    {
        if (settings)
            settings.SetActive(false);
        if (SceneManager.GetActiveScene().buildIndex != 0 && SceneManager.GetActiveScene().buildIndex != 12)
        {
            canPause = true;
            isPaused = false;
            pauseMenu.SetActive(false);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && canPause && pauseMenu && !PlayerHP.player.playerIsDead)
        {
            canPause = false;
            isPaused = true;
            Cursor.lockState = CursorLockMode.None;
            pauseMenu.SetActive(true);
            Time.timeScale = 0f;
        }
    }

    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }


    public void Settings()
    {
        settings.SetActive(true);
    }

    public void ApplySettings()
    {
        settings.SetActive(false);
    }

    public void Continue()
    {
        canPause = true;
        isPaused = false;
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void MainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }

    public void Exit()
    {
        Application.Quit();
    }
}
