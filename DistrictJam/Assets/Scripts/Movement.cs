using Unity.Mathematics;
using UnityEngine;

public class Movement : MonoBehaviour
{
    private Rigidbody rb;
    public Camera cam;

    private float mouseX;
    private float mouseY;
    private float moveSpeed = 5f;
    private float cameraSensitivity = 10f;
    private Vector3 moveDirection;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        // Movement Input
        moveDirection = (transform.forward * Input.GetAxis("Vertical")) + (transform.right * Input.GetAxis("Horizontal"));

        // Mouse Input
        mouseX = Input.GetAxis("Mouse X");
        mouseY = Input.GetAxis("Mouse Y");
    }

    void FixedUpdate()
    {
        Move();
        Rotate();
    }

    private void Move()
    {
        // Movement logic
        Vector3 targetVelocity = moveDirection * moveSpeed;
        targetVelocity.y = rb.linearVelocity.y;

        rb.linearVelocity = targetVelocity;

    }

    private void Rotate()
    {
        // Camera Rotation logic
        this.transform.Rotate(Vector3.up, mouseX * cameraSensitivity);
        cam.transform.Rotate(Vector3.right, -mouseY * cameraSensitivity);
    }
}
