using UnityEngine;

public class killbox : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            Destroy(other.gameObject);
            //other.GetComponent<Movement>().Die();
        }
    }
}
