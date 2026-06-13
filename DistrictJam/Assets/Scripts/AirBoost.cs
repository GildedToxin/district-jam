using UnityEngine;

public class AirBoost : MonoBehaviour
{
    private Rigidbody rb;
    private Movement movement;
    [SerializeField] private GameObject trumpet;

    [SerializeField] private Vector3 trumpetRestingPosition; // No charge position
    [SerializeField] private float trumpetRestingRotationX; // No charge rotation

    [SerializeField] private Vector3 trumpetChargedPosition; // Max charge position
    [SerializeField] private float trumpetChargedRotationX; // Max charge rotation

    private float airBoostCharge = 0f;
    private float airBoostChargeRate = 7f;
    private float airBoostForce = 200f;
    private float airBoostMinForce = 300f;
    private float airBoostMaxForce = 1000f;
    private bool canBoost = true;
    private bool resetBoostScheduled = false;
    private bool isCharging = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        movement = GetComponent<Movement>();
    }

    void Update()
    {
        // Air Boost Input
        if (Input.GetMouseButton(1) && canBoost) // charging
        {
            if (airBoostCharge < 100f)
                airBoostCharge += airBoostChargeRate * Time.deltaTime;
            isCharging = true;
        }
        if (Input.GetMouseButtonUp(1) && canBoost) // let go
        {
            Boost(airBoostCharge);
            airBoostCharge = 0f;
            canBoost = false;
            isCharging = false;
        }
        // Reset the ability to boost after a delay
        if (!canBoost && !resetBoostScheduled)
        {
            resetBoostScheduled = true;
            Invoke("ResetBoost", 1.0f);
        }  

        // Trigger trumpet visual
        if (isCharging)
        {
            if (!trumpet.activeSelf)
                trumpet.SetActive(true);
            MoveTrumpet();
        }
        else
        {
            if (trumpet.activeSelf)
                trumpet.SetActive(false);

            // reset the trumpet's local position and rotation
            trumpet.transform.localPosition = trumpetRestingPosition;
            trumpet.transform.localRotation = Quaternion.Euler(new Vector3(trumpetRestingRotationX, 0, 0));
        }
    }

    private void MoveTrumpet()
    {
        // get the percentage of the charge and lerp the trumpet's position and rotation
        float chargePercentage = airBoostCharge / 100f;
        trumpet.transform.localPosition = Vector3.Lerp(trumpetRestingPosition, trumpetChargedPosition, chargePercentage);
        trumpet.transform.localRotation = Quaternion.Euler(Vector3.Lerp(new Vector3(trumpetRestingRotationX, 0, 0), new Vector3(trumpetChargedRotationX, 0, 0), chargePercentage));
    }

    private void Boost(float charge)
    {
        if (!movement.isGrounded && movement.canMove)
            rb.AddForce(new Vector3(0, Mathf.Clamp(1f * airBoostForce * charge, airBoostMinForce, airBoostMaxForce), 0));
    }

    private void ResetBoost()
    {
        canBoost = true;
        resetBoostScheduled = false;
    }
}
