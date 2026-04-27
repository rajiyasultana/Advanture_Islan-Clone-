using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class SnakeProjectile : MonoBehaviour
{
    [Header("Projectile Settings")]
    public float speed = 5f;
    public float lifeTime = 3f;

    private Rigidbody rb;
    private float timer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        // Ignore gravity so it shoots perfectly straight
        rb.useGravity = false; 
    }

    public void Fire(Vector3 startPosition, Vector3 direction)
    {
        transform.position = startPosition;
        gameObject.SetActive(true);
        timer = 0f;

        // Apply velocity in the target direction
        rb.linearVelocity = direction.normalized * speed;
    }

    private void Update()
    {
        // Auto-destroy after a few seconds so they don't clutter the game
        timer += Time.deltaTime;
        if (timer >= lifeTime)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // If the projectile hits the player, kill them instantly
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerHealthSystem playerHealth = collision.gameObject.GetComponent<PlayerHealthSystem>();
            if (playerHealth != null)
            {
                playerHealth.InstantDeath();
            }
        }

        // Destroy the snake projectile immediately upon hitting anything (ground, player, etc.)
        Destroy(gameObject);
    }
}
