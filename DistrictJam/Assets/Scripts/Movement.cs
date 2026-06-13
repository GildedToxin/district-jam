using Unity.Mathematics;
using UnityEngine;

public class Movement : MonoBehaviour
{
    #region References
    private Rigidbody rb;
    public Camera cam;
    public PauseMenuManager pauseMenu;
    #endregion

    #region Camera & Movement
    private float rotX;
    private float rotY;
    private float moveSpeed = 5f;
    private float cameraSensitivity = 2f;
    private Vector3 moveDirection;
    #endregion

    #region Jump & Physics
    [HideInInspector] public bool isGrounded;
    private float jumpForce = 600f;
    private float gravityScale = 3f;
    private float frictionAmount = 0.5f;
    private float airDrag = 0.01f;
    #endregion

    [HideInInspector] public bool canMove = true;
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
        if (pauseMenu.isPaused == true)
            return;
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

        // Gravity
        Vector3 gravity = -9.81f * gravityScale * Vector3.up;
        rb.AddForce(gravity, ForceMode.Acceleration);
    }

    private void Move()
    {
       


        // Movement logic
        if (isGrounded)
        {
            var grap = FindAnyObjectByType<GrapplingGun>();
            Vector3 toPlayer = rb.position - grap.GrapplePoint;
            float distance = toPlayer.magnitude;

            if (distance >= grap._ropeLength && grap.CurrentGrapplePhase == GrapplingGun.GrapplePhase.Grappling)
            {
                Vector3 dir = toPlayer.normalized;

                float moveAway =
                    Vector3.Dot(rb.linearVelocity, dir);

                if (moveAway > 0f)
                {
                    rb.linearVelocity -= dir * moveAway;
                }
            }

            else
            {
                Vector3 targetVelocity = moveDirection * moveSpeed;
                targetVelocity.y = rb.linearVelocity.y;

                rb.linearVelocity = targetVelocity;
            }
        }
        else if (!isGrounded)
        {
            // Air movement
            rb.AddForce(moveDirection * moveSpeed/15f, ForceMode.VelocityChange);

            // Air drag only in x and z
            rb.AddForce(new Vector3(-rb.linearVelocity.x * airDrag, 0f, -rb.linearVelocity.z * airDrag), ForceMode.VelocityChange);
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
}
