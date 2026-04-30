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
    [Tooltip("How long to wait (in absolute seconds) before showing game over or respawning.")]
    public float delayBeforeGameOver = 1.5f;

    public System.Action<int, int> OnHealthChanged;
    public System.Action OnPlayerDeath; // Triggers the GameOver UI

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

        if (timer >= timeBetweenHealthDrain)
        {
            LoseLife();
            
            if (!isDead)
            {
                timer = 0f; 
                timeBetweenHealthDrain = Mathf.Max(minDrainTime, timeBetweenHealthDrain - 0.1f);
            }
        }
    }

    public void LoseLife()
    {
        if (CurrentLives <= 0 || isDead) return;

        CurrentLives--;

        OnHealthChanged?.Invoke(CurrentLives, MaxLives);

        if (CurrentLives <= 0)
        {
            Die();
        }
    }

    public void TakeDamage(int damageAmount)
    {
        if (CurrentLives <= 0 || isDead) return;

        CurrentLives = Mathf.Max(0, CurrentLives - damageAmount); // Ensure it doesn't go below 0
        OnHealthChanged?.Invoke(CurrentLives, MaxLives);

        if (CurrentLives <= 0)
        {
            Die();
        }
    }

    public void GainHealth(int pointIsFull)
    {
        if (isDead) return;
        if(CurrentLives < MaxLives)
        {
            CurrentLives++;
            timer = 0f;
            OnHealthChanged?.Invoke(CurrentLives, MaxLives);
            Debug.Log($"Gained health! Current Lives: {CurrentLives}/{MaxLives}");
        }
        else
        {
            ScoreSystem playerScore = GetComponent<ScoreSystem>();
            if(playerScore != null)
            {
                playerScore.AddScore(pointIsFull);
                Debug.Log($"Health is full! Added {pointIsFull} points to score. Current Score: {playerScore.Score}");
            }
        }
    }

    public void InstantDeath()
    {
        if (CurrentLives <= 0 || isDead) return;   

        CurrentLives = 0;
        OnHealthChanged?.Invoke(CurrentLives, MaxLives);
        Die();
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        // Stop the game from running
        Time.timeScale = 0f;

        StartCoroutine(DeathRoutine());
    }

    private IEnumerator DeathRoutine()
    {
        // Wait for the absolute delay using realtime
        yield return new WaitForSecondsRealtime(delayBeforeGameOver);

        // Check the GameManager
        if (GameManager.Instance != null)
        {
            // If they have chances left, the GameManager will reload the scene
            // If they don't, it will fall through and we show Game Over
            GameManager.Instance.HandlePlayerDeath();
            
            if (GameManager.Instance.currentChances <= 0)
            {
                OnPlayerDeath?.Invoke(); // Show Game Over Panel
            }
        }
        else
        {
            // Fallback just in case GameManager is missing
            OnPlayerDeath?.Invoke();
        }
    }
}
