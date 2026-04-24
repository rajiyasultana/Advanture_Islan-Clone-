using UnityEngine;
using System.Collections;

public class PlayerHealthSystem : MonoBehaviour
{
    [Header("Health Settings")]
    public int MaxLives { get; private set; } = 11;
    public int CurrentLives { get; private set; }

    [Tooltip("Time in seconds before the player loses 1 life automatically.")]
    public float timeBetweenHealthDrain = 3f;

    [Header("Death Settings")]
    [Tooltip("How long to wait (in absolute seconds) before showing game over.")]
    public float delayBeforeGameOver = 1.5f;

    public System.Action<int, int> OnHealthChanged;
    
    // Add an event specifically for Death so the UI system can listen for it
    public System.Action OnPlayerDeath;

    private float timer = 0f;
    private float minDrainTime = 1f;
    private bool isDead = false;

    void Awake()
    {
        CurrentLives = MaxLives;
    }

    private void Update()
    {
        HandleHealthDrain();
    }

    private void HandleHealthDrain()
    {
        if (CurrentLives <= 0 || isDead) return;

        timer += Time.deltaTime;

        // Use an "if" instead of "while" so we only drain 1 block per frame
        if (timer >= timeBetweenHealthDrain)
        {
            LoseLife();
            
            // Only reset timer if player is still alive, otherwise stop ticking
            if (!isDead)
            {
                timer = 0f; // Reset to 0 instead of subtracting for cleaner pacing
                timeBetweenHealthDrain = Mathf.Max(minDrainTime, timeBetweenHealthDrain - 0.1f);
            }
        }
    }

    public void LoseLife()
    {
        if (CurrentLives <= 0 || isDead) return;

        CurrentLives--;

        // Notify UI Toolkit to update the visual red bars
        OnHealthChanged?.Invoke(CurrentLives, MaxLives);

        Debug.Log($"Player lost a life! Lives remaining: {CurrentLives}");

        if (CurrentLives <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        Debug.Log("Player has lost all lives and died!");

        // Stop the game from running
        Time.timeScale = 0f;

        // Start the Game Over routine using real unscaled time so it continues ticking even while paused
        StartCoroutine(GameOverRoutine());
    }

    private IEnumerator GameOverRoutine()
    {
        // Wait for the absolute delay using realtime (since timescale is 0)
        yield return new WaitForSecondsRealtime(delayBeforeGameOver);

        // Tell the UI script (and any others) that it is time to show the Game over screen
        OnPlayerDeath?.Invoke();
    }

    public void InstantDeath()
    {
        if (CurrentLives <= 0 || isDead) return;   

        CurrentLives = 0;
        
        // Notify UI Toolkit so all health bars disappear at once
        OnHealthChanged?.Invoke(CurrentLives, MaxLives);

        Debug.Log("Player was killed instantly by an enemy or hazard!");
        Die();
    }
}
