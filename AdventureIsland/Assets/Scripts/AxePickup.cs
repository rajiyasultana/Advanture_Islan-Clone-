using UnityEngine;

[RequireComponent(typeof(Collider))]
public class AxePickup : MonoBehaviour
{
    [SerializeField] private AudioClip pickupSound;
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerMovement player = collision.gameObject.GetComponent<PlayerMovement>();

            if (player != null)
            {
                player.EnableThrowing(); // Grant the player the ability to throw
                AudioManager.Instance.PlaySFX(pickupSound); 
                Destroy(gameObject);
            }
        }
    }
}
