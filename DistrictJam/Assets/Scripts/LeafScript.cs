using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using static GrapplingGun;

public class LeafScript : MonoBehaviour
{   
    public GameObject parent;
    public float maxTimer = 3f;
    float currentTimer;
    public float rotateSpeed = 20f;
    bool hit = false;
    private bool rotate = false;
    private float initialRotation = 270f;
    private bool once = true;
    private void Update()
    {
        if (!hit) return;

        currentTimer += Time.deltaTime;
        if(currentTimer > maxTimer)
        {
            //GetComponent<MeshCollider>().enabled = false;
            if (once)
            {
                FindAnyObjectByType<GrapplingGun>().CurrentGrapplePhase = GrapplingGun.GrapplePhase.Retracting;
                once = false;
                var temp = FindAnyObjectByType<GrapplingGun>();
                //temp.Phase();
            }
            Invoke("RotateLeaf", 1f);
        }

        float angle = parent.transform.rotation.eulerAngles.z - -70;
        if (rotate && Mathf.Abs(angle) > 30f)
        {
            parent.transform.rotation = Quaternion.Slerp(parent.transform.rotation, Quaternion.Euler(0, 0, -70), 5f * Time.deltaTime);
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnCollisionEnter(Collision collision)
    {
        hit = true;
    }

    public void RotateLeaf()
    {
        rotate = true;
    }
}
