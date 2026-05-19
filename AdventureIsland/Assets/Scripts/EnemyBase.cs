using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class EnemyBase : MonoBehaviour
{
    [Header("Base Enemy Settings")]
    public int scoreValue = 200;
    public AudioClip EnemydeathSFX;

    protected bool isDead = false;


    protected virtual void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerMovement playerMovement = collision.gameObject.GetComponent<PlayerMovement>();
            PlayerHealthSystem playerHealth = collision.gameObject.GetComponent<PlayerHealthSystem>();

            if (playerHealth != null && playerHealth.HasAngelBuff)
            {
                Die();
                return;
            }


            if (playerMovement != null && playerMovement.HasSkateboard)
            {
                playerMovement.LoseSkateboard();
                Die();
                return;
            }

            if (playerHealth != null)
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
        if (isDead) return;
        isDead = true;
        // Add score to player
        ScoreSystem playerScore = FindObjectOfType<ScoreSystem>();
        if (playerScore != null)
        {
            playerScore.AddScore(scoreValue);
        }

        AudioManager.Instance.PlaySFX(EnemydeathSFX);

        Destroy(gameObject);
    }


}
