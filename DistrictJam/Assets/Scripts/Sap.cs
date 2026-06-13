using UnityEngine;

public class Sap : MonoBehaviour
{
    public float speed;
    public bool canMove = true;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(canMove)
            transform.position -= new Vector3(0, speed * Time.deltaTime, 0);
    }
}
