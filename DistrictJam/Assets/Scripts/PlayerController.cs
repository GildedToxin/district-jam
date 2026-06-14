using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public int firefly = 0;
    public float health = 100;
    public float safeFallVelocity = 10f;
    public float damageMultiplier = 1;
    public bool onWeb = false;
    public WebBox currentWeb = null;

    public bool hasLantern = false;
    public bool hasAcorn = false;
    public bool hasTrumpet = false;
    public bool isDead = false;

    [SerializeField] private GameObject camera;
    [SerializeField] private Vector3 deathInitialPosition;
    [SerializeField] private float deathInitialRotation;
    [SerializeField] private Vector3 deathFinalPosition;
    [SerializeField] private float deathFinalRotation;
    private Vector3 velocity;


    public float maxDeathTimer = 1f;
    public float currentDeathTimer = 0f;
    void FixedUpdate()
    {
        if (isDead)
        {
            DieRagdoll();
        }
    }
    
    public void PickUpItem(bool lantern, bool acorn, bool trumpet)
    {
        if (lantern) hasLantern = true;
        if (acorn) hasAcorn = true;
        if (trumpet) hasTrumpet = true;
    }

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

        if (health <= 0)
        {
            isDead = true;

        }

        velocity = Vector3.zero;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponentInParent<PlayerController>() || collision.gameObject.CompareTag("Lantern")){
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

    private void DieRagdoll() // ragdoll but not really
    {
        float angle = camera.transform.localRotation.eulerAngles.z;
        if (Vector3.Distance(camera.transform.localPosition, deathFinalPosition) > 0.1f)
        {
            camera.transform.localPosition = Vector3.SmoothDamp(camera.transform.localPosition, deathFinalPosition,ref velocity, .5f);
            
            float vel = 2f;
            camera.transform.localRotation = Quaternion.Euler(new Vector3(camera.transform.localRotation.eulerAngles.x, camera.transform.localRotation.eulerAngles.y, 
            Mathf.SmoothDamp(camera.transform.localRotation.eulerAngles.z, deathFinalRotation, ref vel, .5f)));
        }
        else
        {
            if (currentDeathTimer == 0)
                FindAnyObjectByType<HUD>().FadeBlack();
            currentDeathTimer += Time.deltaTime;
            if (currentDeathTimer > maxDeathTimer)
            {
               SceneManager.LoadScene("Game Over");
            }
        }
    }
}
