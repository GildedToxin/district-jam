using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    public TextMeshProUGUI fireflyText;
    public Slider slider;

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
}
