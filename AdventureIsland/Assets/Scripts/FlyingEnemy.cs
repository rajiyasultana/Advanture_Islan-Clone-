using UnityEngine;

public class FlyingEnemy : EnemyBase
{
    [Header("Flying Settings")]
    public float flySpeed = 4f;

    private Rigidbody rb;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (isDead) return;

        rb.linearVelocity = new Vector3(-flySpeed, 0f, 0f);
        
    }

}
