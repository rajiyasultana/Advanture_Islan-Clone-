using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class EggBehavior : MonoBehaviour
{
    [Header("Egg Settings")]
    public GameObject axePrefab;       
    public float throwForceX = 4f;     
    public float throwForceY = 6f;     
    public float timeUntilHatch = 0.8f; 

    private Rigidbody rb;
    private bool isHit = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") && !isHit)
        {
            isHit = true;
            rb.linearVelocity = new Vector3(throwForceX, throwForceY, rb.linearVelocity.z);
            StartCoroutine(BreakEgg());
        }
    }

    private IEnumerator BreakEgg()
    {
        yield return new WaitForSeconds(timeUntilHatch);

        if (axePrefab != null)
        {
            // By default, spawn at the egg's position
            GameObject spawnedAxe = Instantiate(axePrefab, transform.position, Quaternion.identity);

            if (transform.parent != null)
            {
                spawnedAxe.transform.SetParent(transform.parent);
            }
            else 
            {
                // Fallback attempt to find your specific environment parent by name
                GameObject env = GameObject.Find("Environment");
                if(env != null)
                {
                    spawnedAxe.transform.SetParent(env.transform);
                }
            }
        }

        Destroy(gameObject);
    }
}
