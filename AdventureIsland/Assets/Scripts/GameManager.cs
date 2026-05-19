using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Game Settings")]
    public int maxChances = 3;

    [Header("Audio Clips")]
    [SerializeField] private AudioClip gameOverSound;

    public int currentChances;
    public static bool isInitialized = false;

    public System.Action<int> OnChancesChanged;
    public System.Action<int, int> OnShowTransitionPanel;

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

    public void HandlePlayerDeath(int finalScore)
    {
        currentChances--;
        OnChancesChanged?.Invoke(currentChances);

        if(currentChances > 0)
        {
            Debug.Log($"Player Died! Chances left: {currentChances}. Respawning...");

            OnShowTransitionPanel?.Invoke(finalScore, currentChances);

            StartCoroutine(ReloaSceneAfterDelay(3f));
        }
        else
        {
            AudioManager.Instance.PlaySFX(gameOverSound);
            AudioManager.Instance.Stop();
            Debug.Log("GAME OVER! No chances remaining.");
        }
    }

    private IEnumerator ReloaSceneAfterDelay(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);

        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

        AudioManager.Instance.PlayBG();
    }

    public void ResetGame()
    {
        currentChances = maxChances;
        isInitialized = true;
        Time.timeScale = 1f; 
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

        AudioManager.Instance.PlayBG();
    }
}
