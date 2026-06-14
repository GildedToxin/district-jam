using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEditor.Rendering;

public class HUD : MonoBehaviour
{
    public TextMeshProUGUI fireflyText;
    public TextMeshProUGUI infoText;
    public Slider slider;
    public GameObject blackPanel;

    private void Start()
    {
        slider.value = 1;
    }

    public void AddFirefly(int fireflyCount)
    {
        fireflyText.text = fireflyCount.ToString();
    }

    public void TakeDamage(float health, float maxHealth = 100)
    {
        slider.value = health / maxHealth;
    }

    public void SetInfoText(string text)
    {
        infoText.gameObject.SetActive(true);
        infoText.text = text;
    }
    public void FadeBlack()
    {
        StartCoroutine(Fade());
    }
    public IEnumerator Fade()
    {
        float currentalpha = 0;
        //Fade the black panel in using a lerp
        while (true)
        {
            currentalpha = Mathf.Lerp(currentalpha, 1, Time.deltaTime * 5);
            blackPanel.GetComponent<Image>().color = new Color(0, 0, 0, currentalpha);
            yield return null;
        }

    }
}
