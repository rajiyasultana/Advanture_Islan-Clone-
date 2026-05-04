using UnityEngine;

[RequireComponent(typeof(Rigidbody))]

public class Projectile : MonoBehaviour
{
    [Header("Projectile Settings")]
    public float force = 15f; // Force applied to the projectile
    public float lifeTime = 3f; //Disable after 3 seconds

    private Rigidbody rb;
    private float timer;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void Fire(Vector3 startPosition, Vector3 direction)
    {
        transform.position = startPosition;
        gameObject.SetActive(true);
        timer = 0f;

        // Reset velocity before applying force
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        // Apply force in the direction
        rb.AddForce(direction.normalized * force, ForceMode.Impulse);
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= lifeTime)
        {
            ReturnToPool();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        ReturnToPool();
    }

    private void ReturnToPool()
    {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        gameObject.SetActive(false);
    }
}
