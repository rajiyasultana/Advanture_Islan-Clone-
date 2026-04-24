using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class SnailEnemy : MonoBehaviour
{

    [Header("Snail Settings")]
    public float moveSpeed = 1.5f;

    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();

        // Ensure the snail doesn't tip over when moving
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
    }

    void FixedUpdate()
    {
        // Slowly move left across the world
        rb.linearVelocity = new Vector3(-moveSpeed, rb.linearVelocity.y, rb.linearVelocity.z);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Check if the snail collided with the Player
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerHealthSystem playerHealth = collision.gameObject.GetComponent<PlayerHealthSystem>();

            if (playerHealth != null)
            {
                // Instantly kill the player
                playerHealth.InstantDeath();
            }
        }
    }
}
