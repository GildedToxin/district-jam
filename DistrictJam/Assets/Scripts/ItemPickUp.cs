using UnityEngine;

public class ItemPickUp : MonoBehaviour
{
    public float rotationSpeed = 20f;
    public float bobHeight = 0.5f;
    public float bobSpeed = 1f;

    [Range(0f, 0.5f)]
    public float variation = 0.3f;

    private Vector3 startPosition;
    private float bobOffset;

    private float actualRotationSpeed;
    private float actualBobSpeed;
    private float actualBobHeight;

    public bool isLantern;
    public bool isAcorn;
    public bool isTrumpet;

    void Start() { 
    
        startPosition = transform.position;
    }

    void Update()
    {
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.World);

        float newY =
            startPosition.y +
            Mathf.Sin(Time.time * bobSpeed + bobOffset) * bobHeight;

        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Player"))
        {

            collision.GetComponent<PlayerController>().PickUpItem(isLantern, isAcorn, isTrumpet);
            Destroy(gameObject);
        }
    }
}
