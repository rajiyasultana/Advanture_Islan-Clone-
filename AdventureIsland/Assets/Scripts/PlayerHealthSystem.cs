using UnityEngine;

public class PlayerHealthSystem : MonoBehaviour
{
    [Header("Health Settings")]
    public int MaxLives { get; private set; } = 11;
    public int CurrentLives { get; private set; }

    [Tooltip("Time in seconds before the player loses 1 life automatically.")]
    public float timeBetweenHealthDrain = 3f;

    public System.Action<int, int> OnHealthChanged;

    private float timer = 0f;
    private float minDrainTime = 1f;

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
        if (CurrentLives <= 0) return;

        timer += Time.deltaTime;

        while (timer >= timeBetweenHealthDrain)
        {
            LoseLife();
            timer -= timeBetweenHealthDrain;

            // gradually speed up
            timeBetweenHealthDrain = Mathf.Max(minDrainTime, timeBetweenHealthDrain - 0.1f);
        }
    }

    public void LoseLife()
    {
        if (CurrentLives <= 0) return;

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
        Debug.Log("Player has lost all lives and died!");
        // TODO: Trigger game over screen, respawn the player, or reload the scene here.
    }
}
