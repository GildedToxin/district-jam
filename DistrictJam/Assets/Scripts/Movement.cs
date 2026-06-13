using Unity.Mathematics;
using UnityEngine;

public class Movement : MonoBehaviour
{
    private Rigidbody rb;
    public Camera cam;

    private float rotX;
    private float rotY;
    private float moveSpeed = 5f;
    private float cameraSensitivity = 5f;
    private Vector3 moveDirection;
    private bool isGrounded;
    private float jumpForce = 300f;

    public bool canMove = true;
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
        rotX += Input.GetAxis("Mouse X") * cameraSensitivity;
        rotY += Input.GetAxis("Mouse Y") * cameraSensitivity;
        Rotate();

        // Jump Input
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && canMove)
        {
            Jump();
        }

        // Check if the player is grounded
        isGrounded = Physics.Raycast(transform.position, Vector3.down, 1.1f);
    }

    void FixedUpdate()
    {
        if(canMove)
            Move();
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
        rotY = Mathf.Clamp(rotY, -90f, 90f);

        transform.rotation = Quaternion.Euler(0f, rotX, 0f);
        cam.transform.localRotation = Quaternion.Euler(-rotY, 0f, 0f);
    }

    private void Jump()
    {
        // Jumping logic
        rb.AddForce(Vector3.up * jumpForce);
    }
}
