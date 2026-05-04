using UnityEngine;

[RequireComponent (typeof(Collider))]
public class AngelPickup : MonoBehaviour
{
    [Tooltip("How long the player remains invincible in seconds")]
    public float invincibilityDuration = 8f;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerHealthSystem playerHealth = collision.gameObject.GetComponent<PlayerHealthSystem>();

            if (playerHealth != null)
            {
                playerHealth.EnableAngelBuff(invincibilityDuration); // Trigger Invincibility
                Destroy(gameObject);
            }
        }
    }
}
