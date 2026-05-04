using UnityEngine;

[RequireComponent(typeof(Collider))]
public class SkateboardPickup : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerMovement player = collision.gameObject.GetComponent<PlayerMovement>();

            if (player != null)
            {
                player.EnableSkateboard(); // Increase player speed
                Destroy(gameObject);
            }
        }
    }
}
