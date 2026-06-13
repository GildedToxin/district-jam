using UnityEngine;

public class WebBox : MonoBehaviour
{
    public int fireflyCost = 3;
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            CheckWeb(other.GetComponent<PlayerController>());

        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerController>().onWeb = false;
            FindAnyObjectByType<HUD>().infoText.gameObject.SetActive(false);

        }
    }

    public void CheckWeb(PlayerController player)
    {
        if (player.firefly >= fireflyCost)
        {
            FindAnyObjectByType<HUD>().SetInfoText("Press E to burn the web!");
        }
        else
        {
            FindAnyObjectByType<HUD>().SetInfoText("You need " + fireflyCost + " fireflies to burn the web!");
        }
        player.SetWeb(this);
    }

    public void BreakWeb()
    {
        FindAnyObjectByType<HUD>().infoText.gameObject.SetActive(false);
        Destroy(transform.root.gameObject);
    }
}
