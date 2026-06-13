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
        }
        if (Input.GetMouseButtonUp(1) && !movement.isGrounded && movement.canMove) // let go
        {
            Boost(airBoostCharge);
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

    private void Boost(float charge)
    {
        rb.AddForce(new Vector3(0, Mathf.Clamp(1f * airBoostForce * charge, airBoostMinForce, airBoostMaxForce), 0));
    }

    private void ResetBoost()
    {
        canBoost = true;
        resetBoostScheduled = false;
    }
}
