using UnityEngine;

public class SnakeEnemy : EnemyBase
{
    [Header("Attack Settings")]
    public GameObject projectilePrefab;
    public Transform firePoint;     // An empty GameObject at the snake's mouth
    public float fireRate = 1.0f;   // Seconds between shots (reduced for faster firing)
    public Vector3 shootDirection = Vector3.left; // Shoots left by default

    private float fireTimer;
    private Camera mainCamera;
    private Renderer snakeRenderer;

    void Start()
    {
        fireTimer = fireRate;
        mainCamera = Camera.main;
        snakeRenderer = GetComponentInChildren<Renderer>();
    }

    void Update()
    {
        // Only fire if visible in camera view
        if (!IsVisibleInCamera())
            return;

        // Handle shooting timer
        fireTimer -= Time.deltaTime;
        if (fireTimer <= 0f)
        {
            FireProjectile();
            fireTimer = fireRate;
        }
    }

    private bool IsVisibleInCamera()
    {
        if (mainCamera == null || snakeRenderer == null)
            return false;

        return GeometryUtility.TestPlanesAABB(
            GeometryUtility.CalculateFrustumPlanes(mainCamera),
            snakeRenderer.bounds);
    }

    private void FireProjectile()
    {
        if (projectilePrefab == null || firePoint == null)
            return;

        GameObject proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        SnakeProjectile projScript = proj.GetComponent<SnakeProjectile>();
        
        if (projScript != null)
        {
            projScript.Fire(firePoint.position, shootDirection);
        }
    }

    // OnCollisionEnter is handaled in EnemyBase!
}
