using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Quit()
    {
        Application.Quit();
    }
    public void StartGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Tree");
    }
    public void StartTut()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Tutorial");
    }
}
