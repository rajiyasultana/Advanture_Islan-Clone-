using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Game Settings")]
    public int maxChances = 3;

    public int currentChances;
    public static bool isInitialized = false;

    public System.Action<int> OnChancesChanged;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        
        if (!isInitialized)
        {
            currentChances = maxChances;
            isInitialized = true;
        }
    }

    private void Start()
    {
        OnChancesChanged?.Invoke(currentChances);
    }

    public void HandlePlayerDeath()
    {
        currentChances--;
        OnChancesChanged?.Invoke(currentChances);

        if(currentChances > 0)
        {
            Debug.Log($"Player Died! Chances left: {currentChances}. Respawning...");
            
            // Critical setup if your time was slowed to 0 when the player died
            Time.timeScale = 1f; 
            
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        else
        {
            Debug.Log("GAME OVER! No chances remaining.");
        }
    }

    public void ResetGame()
    {
        currentChances = maxChances;
        isInitialized = true;
        Time.timeScale = 1f; 
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
