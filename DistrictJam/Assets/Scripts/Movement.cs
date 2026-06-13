using Unity.Mathematics;
using UnityEngine;

public class Movement : MonoBehaviour
{
    private Rigidbody rb;
    public Camera cam;

    private float mouseX;
    private float mouseY;
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
        mouseX = Input.GetAxis("Mouse X");
        mouseY = Input.GetAxis("Mouse Y");
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
        this.transform.Rotate(Vector3.up, mouseX * cameraSensitivity);
        cam.transform.Rotate(Vector3.right, -mouseY * cameraSensitivity);
        //cam.transform.localEulerAngles = new Vector3(Mathf.Clamp(cam.transform.localEulerAngles.x, -90f, 90f), cam.transform.localEulerAngles.y, cam.transform.localEulerAngles.z);
    }

    private void Jump()
    {
        // Jumping logic
        rb.AddForce(Vector3.up * jumpForce);
    }
}
