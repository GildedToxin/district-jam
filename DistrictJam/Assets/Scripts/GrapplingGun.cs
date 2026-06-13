using UnityEngine;

public class GrapplingGun : MonoBehaviour
{
    [SerializeField] private float _grappleDistance;
    
    public bool _isGrappling;
    public Vector3 _grapplePoint;

    private void Update()
    {
        if(Input.GetMouseButtonDown(0) && !_isGrappling)
        {
            StartGrapple();
        }
        else if (Input.GetMouseButtonUp(0) && _isGrappling)
        {
            //StopGrapple();
        }
    }

    private void StartGrapple()
    {
        if (_isGrappling)
            return;
        RaycastHit hit;
        if (Physics.Raycast(transform.parent.position, transform.parent.forward, out hit, _grappleDistance))
        {
            _isGrappling = true;
            _grapplePoint = hit.point;
            // Implement grappling logic here (e.g., move the player towards the grapple point)
        }
    }
}
