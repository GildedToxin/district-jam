using UnityEngine;

public class RopeRenderer : MonoBehaviour
{
    private LineRenderer _lineRenderer;
    private GrapplingGun _grapplingGun;

    private void Start()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _grapplingGun = GetComponentInParent<GrapplingGun>();
    }

    private void Update()
    {
        if (_grapplingGun.IsGrappling)
        {
            if (!_lineRenderer.enabled)
            {
                _lineRenderer.enabled = true;
            }

            _lineRenderer.SetPosition(0, transform.position);
            _lineRenderer.SetPosition(1, _grapplingGun.grapplePoint);
        }
        else
        {
            _lineRenderer.enabled = false;
        }
    }
}
