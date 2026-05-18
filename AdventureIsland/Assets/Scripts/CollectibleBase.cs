using UnityEngine;

[RequireComponent(typeof(Collider))]
public abstract class CollectibleBase : MonoBehaviour
{
    [Header("Base Collectible Settings")]
    public int scoreValue = 100;

    [Header("Audio Clip")]
    [SerializeField] private AudioClip collectSound;
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

    
    protected virtual void Collect(GameObject player)
    {
        if (scoreValue > 0)
        {
            ScoreSystem playerScore = FindObjectOfType<ScoreSystem>();
            if (playerScore != null)
            {
                playerScore.AddScore(scoreValue);
            }
        }

        if (collectSound != null)
        {
            AudioManager.Instance.PlaySFX(collectSound);
        }

        OnCollected(player);

        DestroyCollectible();
    }

    protected abstract void OnCollected(GameObject player);

    protected virtual void DestroyCollectible()
    {
        Destroy(gameObject);
    }
}
