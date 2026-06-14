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
    public float lv = 0f;

    public bool nextScene;
    public bool win;
    public GameObject[] gameObjects = new GameObject[3];
    void FixedUpdate()
    {
        if (isDead)
        {
            DieRagdoll();
        }
    }

    public void PickUpItem(bool lantern, bool acorn, bool trumpet, bool silver, bool gold)
    {
        if (lantern) hasLantern = true;
        if (acorn) Heal();
        if (silver) SceneManager.LoadScene("MainMenu");
        if (gold)
        {
            SceneManager.LoadScene("Game Over", LoadSceneMode.Additive);
            this.transform.GetChild(1).gameObject.SetActive(false);
            try
            {
                FindAnyObjectByType<Sap>().canMove = false;
            }
            catch { }
            win = true;
            nextScene = true;
        }
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
        if (currentWeb != null && onWeb)
            currentWeb.CheckWeb(this);

        foreach (GameObject obj in gameObjects)
        {
            if (obj != null)
                obj.SetActive(false);
        }

        if (firefly > 0)
            gameObjects[Mathf.Min(3, firefly) - 1].SetActive(true);
    }
    public void SetFirefly(int amount)
    {
        firefly = amount;
        FindAnyObjectByType<HUD>().AddFirefly(firefly);
        if (currentWeb != null && onWeb)
            currentWeb.CheckWeb(this);

        foreach (GameObject obj in gameObjects)
        {
            if (obj != null)
                obj.SetActive(false);
        }
        if (firefly > 0)
            gameObjects[Mathf.Min(3, firefly) - 1].SetActive(true);
    }
    private void Update()
    {
        if (nextScene)
        {
            try
            {
                FindAnyObjectByType<GameOverCanvas>().win = win;
            }
            catch
            {

            }
        }
        lv = GetComponent<Rigidbody>().linearVelocity.y;
        if (currentWeb == null)
            return;

        if (onWeb && firefly >= currentWeb.fireflyCost && Input.GetKeyDown(KeyCode.E))
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
    public void BurnThrow (){
        if (firefly >= currentWeb.fireflyCost)
        {
            SetFirefly(firefly - currentWeb.fireflyCost);
            currentWeb.BreakWeb();
        }
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
        print(GetComponent<Rigidbody>().linearVelocity.y);
        print(lv);
        float damage = Mathf.Max(0, (lv - safeFallVelocity) * damageMultiplier);
        health -= damage;
        FindAnyObjectByType<HUD>().TakeDamage(health);
    }

    public void Heal()
    {
        health = Mathf.Min(100, health + 20);
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
                SceneManager.LoadScene("Game Over", LoadSceneMode.Additive);
                this.transform.GetChild(1).gameObject.SetActive(false);
                try
                {
                    FindAnyObjectByType<Sap>().canMove = false;
                }
                catch { }
                win = false;
                nextScene = true;
            }
        }
    }
}
