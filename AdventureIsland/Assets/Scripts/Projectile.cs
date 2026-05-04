using UnityEngine;

[RequireComponent(typeof(Rigidbody))]

public class Projectile : MonoBehaviour
{
    [Header("Projectile Settings")]
    public float speed = 20f;
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

        // Set velocity to only the projectile speed, ignoring player velocity
        rb.linearVelocity = direction.normalized * speed;
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
        gameObject.SetActive(false);
    }
}
