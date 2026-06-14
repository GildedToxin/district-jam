using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuManager : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenuUI;
    public bool isPaused = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePauseMenu();
        }
    }

    public void TogglePauseMenu()
    {
        if (Time.timeScale == 0f)
        {
            Time.timeScale = 1f; // Resume the game
            pauseMenuUI.SetActive(false); // Hide the pause menu UI
            isPaused = false;

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Time.timeScale = 0f; // Pause the game
            pauseMenuUI.SetActive(true); // Show the pause menu UI
            isPaused = true;

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public void OnResume()
    {
        TogglePauseMenu();
    }

    public void OnQuit()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void Restart()
    {
        SceneManager.LoadScene("Tree");
        Time.timeScale = 1f; // Resume the game
            pauseMenuUI.SetActive(false); // Hide the pause menu UI
            isPaused = false;
    }
}