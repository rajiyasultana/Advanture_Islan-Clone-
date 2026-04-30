using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class EnemyBase : MonoBehaviour
{
    [Header("Base Enemy Settings")]
    public int scoreValue = 200;

    protected virtual void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerHealthSystem playerHealth = collision.gameObject.GetComponent<PlayerHealthSystem>();
            if(playerHealth != null)
            {
                playerHealth.InstantDeath();
            }
        }
        else if (collision.gameObject.CompareTag("Axe"))
        {
            Die();
        }
    }
    protected virtual void Die()
    {
        // Add score to player
        ScoreSystem playerScore = FindObjectOfType<ScoreSystem>();
        if (playerScore != null)
        {
            playerScore.AddScore(scoreValue);
        }
        Destroy(gameObject);
    }
}
