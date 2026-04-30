using UnityEngine;

[RequireComponent(typeof(Collider))]
public abstract class CollectibleBase : MonoBehaviour
{
    [Header("Base Collectible Settings")]
    public int scoreValue = 100;
    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Collect(other.gameObject);
        }
    }

    protected virtual void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            Collect(collision.gameObject);
        }

    }

    /// <summary>
    /// The core logic when the player touches the collectible.
    /// Expected to be overridden by subclasses (HealthPickup, Egg, etc).
    /// </summary>
    /// <param name="player">The player GameObject that collected the item.</param>
    protected virtual void Collect(GameObject player)
    {
        // 1. Add Score (common to almost all pickups)
        if (scoreValue > 0)
        {
            ScoreSystem playerScore = FindObjectOfType<ScoreSystem>();
            if (playerScore != null)
            {
                playerScore.AddScore(scoreValue);
            }
        }

        // 2. Perform unique logic for the specific item (Overridden by children)
        OnCollected(player);

        // 3. Destroy item (unless the specific item prevents it)
        DestroyCollectible();
    }

    /// <summary>
    /// Put the specific functionality in here (e.g., heal player, spawn axe).
    /// </summary>
    protected abstract void OnCollected(GameObject player);

    /// <summary>
    /// Handles destruction, allowing overrides for delays or animations.
    /// </summary>
    protected virtual void DestroyCollectible()
    {
        Destroy(gameObject);
    }
}
