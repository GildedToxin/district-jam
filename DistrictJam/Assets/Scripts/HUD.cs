using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class HUD : MonoBehaviour
{
    public TextMeshProUGUI fireflyText;


public void AddFirefly(int fireflyCount)
    {
        fireflyText.text = fireflyCount.ToString();
    }
}
