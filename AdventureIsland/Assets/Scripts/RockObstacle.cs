using UnityEngine;

[RequireComponent(typeof(Collider))]
public class RockObstacle : EnemyBase
{
    [Header("Rock Settings")]
    public bool isInstantDeath = false;

    public int damageAmount = 2;

    private bool hasTriggered = false;

    protected override void OnCollisionEnter(Collision collision)
    {
        // 1. Behavior for hitting the player
        if (collision.gameObject.CompareTag("Player") && !hasTriggered)
        {
            PlayerHealthSystem healthSystem = collision.gameObject.GetComponent<PlayerHealthSystem>();

            if (healthSystem != null)
            {
                hasTriggered = true; 

                if (isInstantDeath)
                {
                    healthSystem.InstantDeath();
                }
                else
                {
                    healthSystem.TakeDamage(damageAmount);
                    
                }
            }
        }
        
        else if (collision.gameObject.CompareTag("Axe"))
        {
            // In the original Adventure Island, axes bouncing off rocks usually just destroy the axe, 
            // but the rock stays. 
            // If you want the player to be able to BREAK the rocks for points, keep this block:
            
            // Die(); // Uncomment this if you want the axe to destroy the rock and give points
            
            // If you want the rock to be invincible to axes, do nothing here.
            // The Axe script itself should handle destroying itself on impact.
        }
    }
}
