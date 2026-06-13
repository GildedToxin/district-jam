using UnityEngine;

public class GrapplingGun : MonoBehaviour
{
    [SerializeField] private float _grappleDistance;
    
    public bool IsGrappling;
    public Vector3 grapplePoint;
    private Movement _movement;
    private Rigidbody rb;

    private bool _isApplyingGrappleForce;
    private void Start()
    {
        _movement = GetComponentInParent<Movement>();
        rb = GetComponentInParent<Rigidbody>();
    }
    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            StartGrapple();
        }

        if (Input.GetMouseButton(0))
        {
            if (IsGrappling)
            {
                _isApplyingGrappleForce = true;
            }
        }
        else
        {
            IsGrappling = false;
            _movement.enabled = true;
            _isApplyingGrappleForce = false;
        }
        
    }
    private void FixedUpdate()
    {
        if (_isApplyingGrappleForce)
            ApplyGrappleForce();

    }
    private void ApplyGrappleForce()
    {
        Vector3 displacement = grapplePoint - rb.position;
        float theta = Vector3.Angle(displacement, Vector3.up) * Mathf.Deg2Rad;

        float centripetalAcceleration = rb.linearVelocity.sqrMagnitude / displacement.magnitude;
        Vector3 tension = rb.mass * (centripetalAcceleration + Physics.gravity.magnitude * Mathf.Cos(theta)) *displacement.normalized;

        rb.AddForce(tension, ForceMode.Force);
    }
    private void StartGrapple()
    {
        if (IsGrappling)
            return;

        RaycastHit hit;
        if (Physics.Raycast(transform.parent.position, transform.parent.forward, out hit, _grappleDistance))
        {
            IsGrappling = true;
            grapplePoint = hit.point;
            _movement.enabled = false;
            
            rb.linearVelocity = Vector3.zero; // Reset velocity to prevent unwanted momentum when starting the grapple

        }
    }
}
