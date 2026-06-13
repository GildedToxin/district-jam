using UnityEngine;

public class AirBoost : MonoBehaviour
{
    private Rigidbody rb;
    private Movement movement;
    [SerializeField] private GameObject trumpet;

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
            if (trumpet.activeSelf)
                trumpet.SetActive(false);
    }

    private void MoveTrumpet()
    {
        
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
