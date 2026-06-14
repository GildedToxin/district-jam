using System.Collections;
using UnityEngine;

public class LeafScript : MonoBehaviour
{
    public float maxTimer = 3f;
    float currentTimer;
    public float rotateSpeed = 20f;
    bool hit = false;
    private void Update()
    {
        if (!hit) return;

        currentTimer += Time.deltaTime;
        if(currentTimer > maxTimer)
        {
            GetComponent<MeshCollider>().enabled = false;
            StartCoroutine(RotateLeaf());
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnCollisionEnter(Collision collision)
    {
        hit = true;
    }

    public IEnumerator RotateLeaf()
    {
        while (transform.parent.rotation.y < 360)
        {
            transform.parent.Rotate(Vector3.left, rotateSpeed * Time.deltaTime);
            yield return null;
        }
    }
}
