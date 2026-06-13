using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public int firefly = 0;
    public float health = 100;
    public float safeFallVelocity = 10f;
    public float damageMultiplier = 1;



    public void AddFirefly()
    {
        firefly++;
        FindAnyObjectByType<HUD>().AddFirefly(firefly);
    }
    private void OnCollisionEnter(Collision collision)
    {
        var hitTop = false;
        foreach (ContactPoint contact in collision.contacts)
        {
            // Surface is facing upward
            if (Vector3.Dot(contact.normal, Vector3.up) > 0.9f)
            {

                hitTop = true;

            }
        }
        float damage = Mathf.Max(0, (-GetComponent<Rigidbody>().linearVelocity.y - safeFallVelocity) * damageMultiplier);
        health -= damage;
        FindAnyObjectByType<HUD>().TakeDamage(health);
    }
}
