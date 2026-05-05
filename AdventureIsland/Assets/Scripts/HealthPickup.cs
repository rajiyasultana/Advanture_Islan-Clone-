using UnityEngine;

[RequireComponent(typeof(Collider))]
public class HealthPickup : CollectibleBase
{
    [Header("Health Picup Settings")]
    public int points = 100;


    protected override void OnCollected(GameObject player)
    {
        PlayerHealthSystem playerHealth = player.GetComponent<PlayerHealthSystem>();
        if (playerHealth != null)
        {
            playerHealth.GainHealth(points);
        }
    }
}
