using UnityEngine;
using System;

public class GrapplingGun : MonoBehaviour
{

    public event Action<GrapplePhase> GrapplePhaseChanged;

    public Vector3 GrapplePoint { get; private set; }
    public GrapplePhase CurrentGrapplePhase;

    public enum GrapplePhase
    {
        Waiting,
        Launching,
        Grappling,
        Retracting
    }

    [SerializeField] private float _reelInAcceleration;
    [SerializeField] private float _retractionTime;
    [SerializeField] public Transform _launcherTransform;

    private Movement _playerMovement;
    private Rigidbody _playerRigidbody;

    private bool _isApplyingGrappleForces;

    public float _ropeLength;
    private bool _isRopeInTension;

    private float _reelInSpeed;
    private bool _isReelingIn;

    private Vector3 _launcherOffset;
    private float _reatactionTimer;

    private bool _holdingReelIn;
    public float minRopeLength = 10f;
    public float maxRopeLength = 80f;
    public float currentRopeLength;

    private bool _holdingLetOut;

    public MeshRenderer lanternMesh;
    public GameObject flies;
    public void LaunchFinished(bool isLaunchSuccessful, Vector3 grapplePoint)
    {
        if (CurrentGrapplePhase != GrapplePhase.Launching)
        {
            return;
        }

        GrapplePoint = grapplePoint;

        if (isLaunchSuccessful)
        {
            StartGrapple();
            _launcherTransform.localRotation = Quaternion.Euler(0, 0, 0);
        }
        else
        {
            CurrentGrapplePhase = GrapplePhase.Retracting;
            GrapplePhaseChanged?.Invoke(GrapplePhase.Launching);
        }
    }

    private void Start()
    {
        lanternMesh.enabled = false;
        flies.SetActive(false);
        _playerMovement = transform.parent.GetComponentInParent<Movement>();
        _playerRigidbody = transform.parent.GetComponentInParent<Rigidbody>();

        CurrentGrapplePhase = GrapplePhase.Waiting;

        _launcherOffset = _launcherTransform.localPosition;
    }

    private void Update()
    {
        if(FindAnyObjectByType<PlayerController>().hasLantern == false)
            return;
        _holdingLetOut = Input.GetKey(KeyCode.F);
        _holdingReelIn = Input.GetKey(KeyCode.R);
        currentRopeLength = (_launcherTransform.position- _playerRigidbody.position).magnitude;
        if (Input.GetMouseButtonDown(0) && CurrentGrapplePhase == GrapplePhase.Waiting)
        {
            lanternMesh.enabled = true;
            flies.SetActive(true);
            CurrentGrapplePhase = GrapplePhase.Launching;
            GrapplePhaseChanged?.Invoke(GrapplePhase.Waiting);
        }

        if (CurrentGrapplePhase == GrapplePhase.Grappling)
        {
            if (Input.GetMouseButton(0))
            {
                _isApplyingGrappleForces = true;

                _isRopeInTension = _ropeLength * _ropeLength < (GrapplePoint - _playerRigidbody.position).sqrMagnitude;
            }
            else
            {
                _isApplyingGrappleForces = false;
                _playerMovement.canMove = true;

                CurrentGrapplePhase = GrapplePhase.Retracting;
                GrapplePhaseChanged?.Invoke(GrapplePhase.Grappling);
            }
        }

        if (CurrentGrapplePhase == GrapplePhase.Retracting)
        {
            if (_reatactionTimer < _retractionTime)
            {
                _reatactionTimer += Time.deltaTime;

                float t = _reatactionTimer / _retractionTime;
                Vector3 finalPosition = _playerRigidbody.position + _launcherOffset;

                _launcherTransform.position = Vector3.Lerp(GrapplePoint, finalPosition, t);
            }
            else
            {
                ResetGrapplingGun();
            }
        }
    }
    public void Phase()
    {
        GrapplePhaseChanged?.Invoke(GrapplePhase.Waiting);
    }
    private void FixedUpdate()
    {
        if (_isApplyingGrappleForces )
        {
            ApplyGrappleForces();

            if (Vector3.Dot(_playerRigidbody.linearVelocity, GrapplePoint - _playerRigidbody.position) <= 0 && _isRopeInTension)
            {
                TugPlayer();
            }

            if (_ropeLength > minRopeLength && _isReelingIn && _holdingReelIn)
            {
                _reelInSpeed += _reelInAcceleration * Time.fixedDeltaTime;
                _ropeLength -= _reelInSpeed * Time.fixedDeltaTime;
            }
            else if (_holdingLetOut)
            {
                _reelInSpeed = 0f;

                _ropeLength += _reelInAcceleration * Time.fixedDeltaTime;
                _ropeLength = Mathf.Clamp(_ropeLength, minRopeLength, maxRopeLength);
            }
            else
            {
                _reelInSpeed = 0f;
            }
        }
    }

    private void StartGrapple()
    {
        //_playerMovement.canMove = false;
        _ropeLength = (GrapplePoint - _playerRigidbody.position).magnitude;
        _isReelingIn = true;
        _reelInSpeed = 0;

        CurrentGrapplePhase = GrapplePhase.Grappling;
        GrapplePhaseChanged?.Invoke(GrapplePhase.Launching);
    }

    private void ApplyGrappleForces()
    {
        Vector3 direction = (GrapplePoint - _playerRigidbody.position).normalized;
        float theta = Vector3.Angle(direction, Vector3.up) * Mathf.Deg2Rad;

        float centripetalAcceleration = Mathf.Min(_playerRigidbody.linearVelocity.sqrMagnitude / Mathf.Max(_ropeLength, 0.1f), 50f);
        Vector3 tension = _playerRigidbody.mass * (centripetalAcceleration + Physics.gravity.magnitude * Mathf.Cos(theta)) * direction;

        if (_isRopeInTension )
        {
            if (_isReelingIn)
            {
                _playerRigidbody.AddForce(_playerRigidbody.mass * _reelInAcceleration * direction);
            }

            _playerRigidbody.AddForce(tension, ForceMode.Force);
        }
    }

    private void TugPlayer()
    {
        Vector3 direction = (GrapplePoint - _playerRigidbody.position).normalized;

        Vector3 tangentialVelocity = Vector3.ProjectOnPlane(_playerRigidbody.linearVelocity, direction);
        _playerRigidbody.linearVelocity = tangentialVelocity;

        _isRopeInTension = true;

        Vector3 toPlayer = _playerRigidbody.position - GrapplePoint;
        Vector3 corrected = GrapplePoint + toPlayer.normalized * _ropeLength;

        Vector3 correctionVelocity = (corrected - _playerRigidbody.position) / Time.fixedDeltaTime;

        _playerRigidbody.linearVelocity = Vector3.ProjectOnPlane(
            _playerRigidbody.linearVelocity + correctionVelocity,
            direction
        );
    }

    private void ResetGrapplingGun()
    {
        _launcherTransform.parent = transform;
        _launcherTransform.localPosition = _launcherOffset;
        _launcherTransform.localRotation = Quaternion.Euler(0, 0, 0);
        _reatactionTimer = 0;
        CurrentGrapplePhase = GrapplePhase.Waiting;
        GrapplePhaseChanged?.Invoke(GrapplePhase.Retracting);
        lanternMesh.enabled = false;
        flies.SetActive(false);
    }


}