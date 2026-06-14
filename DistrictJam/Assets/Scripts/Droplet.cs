using UnityEngine;

public class Droplet : MonoBehaviour
{
    public GameObject droplet;
    private float timeToFall = 1f;
    
    void Start()
    {
        
    }

    void Update()
    {
        drop();
    }

    private void drop()
    {
        if (Vector3.Distance(droplet.transform.position, this.transform.position) > 1f)
            droplet.transform.position = droplet.transform.position + Vector3.Lerp(droplet.transform.position, this.transform.position, timeToFall * Time.deltaTime);
    }
}
