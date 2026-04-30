using UnityEngine;

public class SnakeEnemy : EnemyBase
{
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
            GameObject proj = Instantiate(projectilePrefab);
            
            SnakeProjectile projScript = proj.GetComponent<SnakeProjectile>();
            if (projScript != null)
            {
                projScript.Fire(firePoint.position, shootDirection);
            }
        }
    }

    // OnCollisionEnter is handaled in EnemyBase!
}
