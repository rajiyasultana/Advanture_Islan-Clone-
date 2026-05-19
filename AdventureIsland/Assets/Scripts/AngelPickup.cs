using UnityEngine;

[RequireComponent (typeof(Collider))]
public class AngelPickup : MonoBehaviour
{
    [SerializeField] private float invincibilityDuration = 8f;

    [Header("Audio")]
    [SerializeField] private AudioClip pickupSound;
    [SerializeField] private AudioClip angelEffectSound;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerHealthSystem playerHealth = collision.gameObject.GetComponent<PlayerHealthSystem>();

            if (playerHealth != null)
            {
                playerHealth.EnableAngelBuff(invincibilityDuration); // Trigger Invincibility
                AudioManager.Instance.PlaySFX(pickupSound);
                AudioManager.Instance.PlaySFX(angelEffectSound);
                Destroy(gameObject);
            }
        }
    }
}
