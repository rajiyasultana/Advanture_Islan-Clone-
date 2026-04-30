using UnityEngine;

[RequireComponent(typeof(Collider))]
public class RockObstacle : MonoBehaviour
{
    [Header("Damage Settings")]
    [Tooltip("If checked, touching this rock instantly kills the player.")]
    public bool isInstantDeath = false;

    [Tooltip("How many energy bars are lost (only applies if Is Instant Death is false).")]
    public int damageAmount = 2;

    private bool hasTriggered = false;

    private void OnCollisionEnter(Collision collision)
    {
        // Only trigger once when the player physically hits the rock
        if (collision.gameObject.CompareTag("Player") && !hasTriggered)
        {
            PlayerHealthSystem healthSystem = collision.gameObject.GetComponent<PlayerHealthSystem>();

            if (healthSystem != null)
            {
                hasTriggered = true; // Stop it from registering multiple times instantly

                if (isInstantDeath)
                {
                    // Big Rock: Kill them instantly!
                    healthSystem.InstantDeath();
                }
                else
                {
                    // Small Rock: Just take away a set number of energy bars
                    healthSystem.TakeDamage(damageAmount);
                    
                    // Optional: Destroy small rocks after tripping over them so they don't block the path permanently
                    Destroy(gameObject);
                }
            }
        }
    }
}
