using Unity.Mathematics;
using UnityEngine;

public class Movement : MonoBehaviour
{
    private Rigidbody rb;
    private Vector3 velocity;
    private float moveX;
    private float moveZ;
    private float moveSpeed = 5f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        // Movement Input
        moveX = Input.GetAxis("Horizontal");
        moveZ = Input.GetAxis("Vertical");

        
    }

    void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        // Movement logic

        if (moveX > 0.1f || moveX < -0.1f)
        {
            velocity.x = moveX * moveSpeed * Time.fixedDeltaTime;
        }
        else velocity.x = 0f;

        if (moveZ > 0.1f || moveZ < -0.1f)
        {
            velocity.z = moveZ * moveSpeed * Time.fixedDeltaTime;
        }
        else velocity.z = 0f;

        rb.Move(this.transform.position + velocity, Quaternion.identity );
    }
}
