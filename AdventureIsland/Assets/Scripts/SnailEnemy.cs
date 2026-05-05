using UnityEngine;

public class SnailEnemy : EnemyBase
{
    [Header("Snail Settings")]
    public float moveSpeed = 1.5f;

    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
    }

    void FixedUpdate()
    {
        rb.linearVelocity = new Vector3(-moveSpeed, rb.linearVelocity.y, rb.linearVelocity.z);
    }

}
