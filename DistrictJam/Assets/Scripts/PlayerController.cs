using UnityEngine;

public class PlayerController : MonoBehaviour
{
public int firefly = 0;




    public void AddFirefly()
    {
        firefly++;
        FindAnyObjectByType<HUD>().AddFirefly(firefly);
    }

}
