using UnityEngine;

public class GrappleLauncher : MonoBehaviour
{

    [SerializeField] private float _launchSpeed;
    [SerializeField] private float _maxLaunchTime;

    public bool _isLaunched;
    private float _launchTimer;

    private SphereCollider _sphereCollider;
    private Rigidbody _rigidbody;
    public GrapplingGun _grapplingGun;

    public LayerMask _grappleLayerMask;
    [SerializeField] private float gravityScale = 3f;
    private void Start()
    {
        _sphereCollider = GetComponent<SphereCollider>();
        _rigidbody = GetComponent<Rigidbody>();
        //_grapplingGun = GetComponent<GrapplingGun>();

        _grapplingGun.GrapplePhaseChanged += OnGrapplePhaseChanged;
    }

    private void OnGrapplePhaseChanged(GrapplingGun.GrapplePhase previousGrapplePhase)
    {
        if (_grapplingGun.CurrentGrapplePhase == GrapplingGun.GrapplePhase.Launching && previousGrapplePhase == GrapplingGun.GrapplePhase.Waiting)
        {
            Launch();
        }
    }

    private void Launch()
    {
        _isLaunched = true;
        _launchTimer = _maxLaunchTime;
        _sphereCollider.enabled = true;
        _rigidbody.isKinematic = false;

        _rigidbody.AddForce(_rigidbody.mass * _launchSpeed * Camera.main.transform.forward, ForceMode.Impulse);

        transform.parent = null;
    }

    private void EndLaunch(bool isLaunchSuccessful)
    {
        _isLaunched = false;
        _sphereCollider.enabled = false;
        _rigidbody.isKinematic = true;

        _grapplingGun.LaunchFinished(isLaunchSuccessful, transform.position);
    }

    private void FixedUpdate()
    {
        if (!_isLaunched) return;

        // --- 1. Apply gravity every frame ---
        Vector3 gravity = Physics.gravity * gravityScale;
        _rigidbody.AddForce(gravity, ForceMode.Acceleration);

        // --- 2. Rope constraint ---
        Vector3 anchorPoint = _grapplingGun.transform.position; // hook / fire point
        Vector3 toObject = _rigidbody.position - anchorPoint;

        float distance = toObject.magnitude;
        float maxDistance = _grapplingGun.maxRopeLength;

        if (distance > maxDistance)
        {
            Vector3 dir = toObject / distance;

            // Snap back onto rope boundary (hard constraint)
            _rigidbody.position = anchorPoint + dir * maxDistance;

            // Remove velocity pushing further outward
            Vector3 vel = _rigidbody.linearVelocity;

            // Remove radial component only (keeps swing tangential motion)
            vel -= Vector3.Dot(vel, dir) * dir;

            _rigidbody.linearVelocity = vel;
        }
    }
    private void Update()
    {
        if (_isLaunched)
        {
            _launchTimer -= Time.deltaTime;

            //if (_launchTimer < 0)
            //{
            //    EndLaunch(false);
            //}

            if (Input.GetMouseButtonUp(0))
            {
                EndLaunch(false);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        var hitTop = false;
        foreach (ContactPoint contact in collision.contacts)
        {
            // Surface is facing upward
            if (Vector3.Dot(contact.normal, Vector3.up) > 0.9f)
            {
                Debug.Log("Hit the top!");
                hitTop = true;

            }
        }


        if (collision.gameObject.layer == LayerMask.NameToLayer("Firefly"))
        {
            Destroy(collision.gameObject);
            FindAnyObjectByType<PlayerController>().AddFirefly();
             return;
        }
        if (!hitTop)
        {
            return;
        }
        if (collision.gameObject.layer != LayerMask.NameToLayer("Grapple"))
        {
            return;
        }
      

        if (collision.gameObject.GetComponent<Movement>() != null || collision.gameObject.GetComponentInParent<Movement>())
        {
            return;
        }
        if (_isLaunched)
        {
            transform.position = collision.GetContact(0).point;
            transform.right = -collision.GetContact(0).normal;

            EndLaunch(true);
        }
    }

    private void OnDestroy()
    {
        _grapplingGun.GrapplePhaseChanged -= OnGrapplePhaseChanged;
    }

}