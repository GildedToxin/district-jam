using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuManager : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenuUI;
    public bool isPaused = false;

    public void TogglePauseMenu()
    {
        if (Time.timeScale == 0f)
        {
            Time.timeScale = 1f; // Resume the game
            pauseMenuUI.SetActive(false); // Hide the pause menu UI
            // lock cursor to the game and make it invisible
            //Cursor.lockState = CursorLockMode.Locked;
            //Cursor.visible = false;
        }
        else
        {
            Time.timeScale = 0f; // Pause the game
            pauseMenuUI.SetActive(true); // Show the pause menu UI
            // unlink cursor from the game and make it visible
            //Cursor.lockState = CursorLockMode.None;
            //Cursor.visible = true;
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
}