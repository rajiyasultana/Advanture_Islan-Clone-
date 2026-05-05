using UnityEngine;
using System.Collections;

public class PlayerHealthSystem : MonoBehaviour
{
    [Header("Health Settings")]
    public int MaxLives { get; private set; } = 11;
    public int CurrentLives { get; private set; }

    public float timeBetweenHealthDrain = 5f;

    [Header("Death Settings")]
    public float delayBeforeGameOver = 1.5f;

    public System.Action<int, int> OnHealthChanged;
    public System.Action OnPlayerDeath; // Triggers the GameOver UI

    private float timer = 0f;
    private bool isDead = false;

    // Buff properties
    public bool HasAngelBuff { get; private set; } = false;
    private Coroutine angelCoroutine;
    private PlayerMovement playerMovement;

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
            }
        }
    }

    public void EnableAngelBuff(float duration)
    {
        if (angelCoroutine != null) StopCoroutine(angelCoroutine);
        angelCoroutine = StartCoroutine(AngelRoutine(duration));
    }

    private IEnumerator AngelRoutine(float duration)
    {
        HasAngelBuff = true;
        Debug.Log("Angel invincibility active!");
        yield return new WaitForSeconds(duration);
        HasAngelBuff = false;
        Debug.Log("Angel invincibility ended.");
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

        CurrentLives = Mathf.Max(0, CurrentLives - damageAmount); 
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
            int finalScore = 0;
            ScoreSystem playerScore = GetComponent<ScoreSystem>();
            if(playerScore != null)
            {
                finalScore = playerScore.Score;
            }
            GameManager.Instance.HandlePlayerDeath(finalScore);
            
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
