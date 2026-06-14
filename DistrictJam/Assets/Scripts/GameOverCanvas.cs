using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverCanvas : MonoBehaviour
{
    public GameObject winPanel;
    public GameObject losePanel;


    public TextMeshProUGUI text;
    public GameObject thankyou;

    public bool win = false;
    public float maxTimer = 5f;
    public float currentTimer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        winPanel.SetActive(false);
        losePanel.SetActive(false);
        thankyou.SetActive(false);
        text.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
       currentTimer += Time.deltaTime;
       if(currentTimer > maxTimer)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.Confined;
            if (win)
            {
                text.gameObject.SetActive(true);
                text.text = "Aw Sap, you win!";
                winPanel.SetActive(true);
                thankyou.SetActive(true);
            }
            else
            {
                text.gameObject.SetActive(true);
                text.text = "Aw Nuts, you lose!";
                losePanel.SetActive(true);
            }
        }
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
    public void Quit()
    {
        Application.Quit();
    }
    public void Restart()
    {
        SceneManager.LoadScene("Tree");
    }
}
