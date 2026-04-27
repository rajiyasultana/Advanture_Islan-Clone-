using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class SnakeEnemy : MonoBehaviour
{
    [Header("Enemy Settings")]
    public int scoreValue = 200;

    [Header("Attack Settings")]
    public GameObject projectilePrefab;
    public Transform firePoint;     // An empty GameObject at the snake's mouth
    public float fireRate = 2.0f;   // Seconds between shots
    public Vector3 shootDirection = Vector3.left; // Shoots left by default

    private float fireTimer;

    void Update()
    {
        // Handle shooting timer
        fireTimer -= Time.deltaTime;
        if (fireTimer <= 0f)
        {
            FireProjectile();
            fireTimer = fireRate; // Reset the timer
        }
    }

    private void FireProjectile()
    {
        if (projectilePrefab != null && firePoint != null)
        {
            // Spawn the projectile
            GameObject proj = Instantiate(projectilePrefab);
            
            // Get its script and fire it
            SnakeProjectile projScript = proj.GetComponent<SnakeProjectile>();
            if (projScript != null)
            {
                // Fire from the firePoint toward the specified direction
                projScript.Fire(firePoint.position, shootDirection);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Check if the Player physically touches the snake
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerHealthSystem playerHealth = collision.gameObject.GetComponent<PlayerHealthSystem>();

            if (playerHealth != null)
            {
                // Instantly kill the player
                playerHealth.InstantDeath();
            }
        }
        // Check if the Player's Axe hits the snake
        else if (collision.gameObject.CompareTag("Axe"))
        {
            ScoreSystem playerScore = FindObjectOfType<ScoreSystem>();
            if (playerScore != null)
            {
                // Give points!
                playerScore.AddScore(scoreValue);
            }
            
            // Destroy the snake immediately
            Destroy(gameObject);
        }
    }
}
