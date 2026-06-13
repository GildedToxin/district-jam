using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public int firefly = 0;
    public float health = 100;
    public float safeFallVelocity = 10f;
    public float damageMultiplier = 1;
    public bool onWeb = false;
    public WebBox currentWeb = null;


    public void SetWeb(WebBox web)
    {
        onWeb = true;
        currentWeb = web;
    }
    public void AddFirefly()
    {
        firefly++;
        FindAnyObjectByType<HUD>().AddFirefly(firefly);
        if(currentWeb != null && onWeb)
            currentWeb.CheckWeb(this);
    }
    public void SetFirefly(int amount)
    {
        firefly = amount;
        FindAnyObjectByType<HUD>().AddFirefly(firefly);
        if (currentWeb != null && onWeb)
            currentWeb.CheckWeb(this);
    }
    private void Update()
    {
        if (currentWeb == null) 
            return;

        if(onWeb && firefly >= currentWeb.fireflyCost && Input.GetKeyDown(KeyCode.E))
        {
            SetFirefly(firefly - currentWeb.fireflyCost);
            currentWeb.BreakWeb();
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponentInParent<PlayerController>()){
            return;
        }
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
