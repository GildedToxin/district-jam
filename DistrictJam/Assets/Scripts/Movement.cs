using Unity.Mathematics;
using UnityEngine;

public class Movement : MonoBehaviour
{
    #region References
    private Rigidbody rb;
    public Camera cam;
    #endregion

    #region Camera & Movement
    private float rotX;
    private float rotY;
    private float moveSpeed = 5f;
    private float cameraSensitivity = 2f;
    private Vector3 moveDirection;
    #endregion

    #region Air Boost
    private float airBoostCharge = 0f;
    private float airBoostChargeRate = 7f;
    private float airBoostForce = 200f;
    private float airBoostMinForce = 300f;
    private float airBoostMaxForce = 1000f;
    #endregion

    #region Jump & Physics
    private bool isGrounded;
    private float jumpForce = 600f;
    private bool canBoost = true;
    private bool resetBoostScheduled = false;
    private float gravityScale = 3f;
    private float frictionAmount = 0.5f;
    #endregion

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

        // Air Boost Input
        if (Input.GetMouseButton(1) && canMove && canBoost) // charging
        {
            if (airBoostCharge < 100f)
                airBoostCharge += airBoostChargeRate * Time.deltaTime;
        }
        if (Input.GetMouseButtonUp(1) && !isGrounded && canMove) // let go
        {
            AirBoost(airBoostCharge);
            airBoostCharge = 0f;
            canBoost = false;
        }

        // Reset the ability to boost after a delay
        if (!canBoost && !resetBoostScheduled)
        {
            resetBoostScheduled = true;
            Invoke("ResetBoost", 1.0f);
        }
    }

    void FixedUpdate()
    {
        if(canMove)
            Move();

        // Gravity
        Vector3 gravity = -9.81f * gravityScale * Vector3.up;
        rb.AddForce(gravity, ForceMode.Acceleration);
    }

    private void Move()
    {
        // Movement logic
        if (isGrounded)
        {
            Vector3 targetVelocity = moveDirection * moveSpeed;
            targetVelocity.y = rb.linearVelocity.y;

            rb.linearVelocity = targetVelocity;
        }

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

    private void AirBoost(float charge)
    {
        rb.AddForce(new Vector3(0, Mathf.Clamp(1f * airBoostForce * charge, airBoostMinForce, airBoostMaxForce), 0));
    }

    private void ResetBoost()
    {
        canBoost = true;
        resetBoostScheduled = false;
    }
}
