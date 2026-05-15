using System.Collections;
using UnityEngine;

public class FlyingEnemy : EnemyBase
{
    [Header("Flying Settings")]
    public float flySpeed = 4f;
    public float deathDestroyDelay = 2f;

    private Rigidbody rb;
    private Collider coll;
    private Animator childAnimator;
    private bool isDead = false;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        coll = GetComponent<Collider>();
        childAnimator = GetComponentInChildren<Animator>();

        if (rb != null)
        {
            rb.useGravity = false;
        }
    }

    void FixedUpdate()
    {
        if (isDead) return;

        if (rb != null)
        {
            rb.linearVelocity = new Vector3(-flySpeed, 0f, 0f);
        }
    }

    protected override void OnCollisionEnter(Collision collision)
    {
        base.OnCollisionEnter(collision); // Still do the EnemyBase logic (InstantDeath the player / check Axe)


        // If it collides with the player, make the bird fall!
        if (collision.gameObject.CompareTag("Player"))
        {
            TriggerFallEffects();
        }
    }

    protected override void Die()
    {
        // Add score to player only when killed by the Axe (Die is called from EnemyBase when Axe hits)
        ScoreSystem playerScore = FindObjectOfType<ScoreSystem>();
        if (playerScore != null && !isDead)
        {
            playerScore.AddScore(scoreValue);
        }

        TriggerFallEffects();
    }

    private void TriggerFallEffects()
    {
        if (isDead) return;
        isDead = true;

        // 1. Play death animation on the child model
        if (childAnimator != null)
        {
            childAnimator.SetTrigger("IsDie");
        }

        // 2. Disable collider so it doesn't interact anymore as it falls
        if (coll != null)
        {
            coll.enabled = false;
        }

        // 3. Fall mechanism
        if (rb != null)
        {
            rb.useGravity = true; // Turn gravity back on

            // Stop left movement, push slightly down, and spin
            rb.linearVelocity = new Vector3(0f, -2f, 0f);
            rb.angularVelocity = new Vector3(0f, 0f, 10f); 
        }

        // 4. Destroy after falling off screen
        Destroy(gameObject, deathDestroyDelay);
    }

    


}
