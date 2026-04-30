using UnityEngine;

[RequireComponent(typeof(Collider))]
public class HealthPickup : MonoBehaviour
{
    [Header("PicupSettings")]
    public int points = 100;


    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            PlayerHealthSystem playerHealth = other.GetComponent<PlayerHealthSystem>();
            if (playerHealth != null)
            {
                playerHealth.GainHealth(points);
                Destroy(gameObject);
            }
        }
    }
}
