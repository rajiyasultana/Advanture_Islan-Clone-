using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System.Linq;

public class HealthUI : MonoBehaviour
{
    [Header("UI Setup")]
    public UIDocument uiDocument;

    [Header("Player References")]
    public PlayerHealthSystem playerHealth;
    public ScoreSystem playerScore;

    private List<VisualElement> bars = new List<VisualElement>();

    
    // UI Elements
    private VisualElement gameOverPanel;
    private VisualElement gameUI;
    private Label scoreLabel;
    private Label chancesLabel; // <--- Target the '3 lives' label

    void Start()
    {
        if (uiDocument == null || playerHealth == null) return;

        var root = uiDocument.rootVisualElement;

        gameUI = root.Q<VisualElement>("GameUI");
        gameOverPanel = root.Q<VisualElement>("GameOverPanel");
        
        if (gameOverPanel != null) gameOverPanel.style.display = DisplayStyle.None; 

        var container = root.Q<VisualElement>("healthBars");
        if (container != null) bars = container.Query<VisualElement>(className: "health-bar").ToList();
        
        scoreLabel = root.Q<Label>(className: "score-text");
        
        // Grab the chance label
        chancesLabel = root.Q<Label>(className: "life-text");

        // System Events
        playerHealth.OnHealthChanged += UpdateHealth;
        playerHealth.OnPlayerDeath += ShowGameOverScreen;
        UpdateHealth(playerHealth.CurrentLives, playerHealth.MaxLives);

        if (playerScore != null)
        {
            playerScore.OnScoreChanged += UpdateScore;
            UpdateScore(playerScore.Score); 
        }

        // Subscribe to GameManager to update the "3 lives" text
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnChancesChanged += UpdateChances;
            UpdateChances(GameManager.Instance.currentChances);
        }
    }

    void UpdateHealth(int current, int max)
    {
        int lostLives = max - current;
        for (int i = 0; i < bars.Count; i++)
        {
            bars[i].style.display = (i < lostLives) ? DisplayStyle.None : DisplayStyle.Flex;
        }
    }

    void UpdateScore(int currentScore)
    {
        if (scoreLabel != null) scoreLabel.text = currentScore.ToString("D6"); 
    }

    void UpdateChances(int remainingChances)
    {
        if (chancesLabel != null) chancesLabel.text = remainingChances.ToString();
    }

    private void ShowGameOverScreen()
    {
        if (gameOverPanel != null) gameOverPanel.style.display = DisplayStyle.Flex;
        if (gameUI != null) gameUI.style.display = DisplayStyle.None;
    }

    private void OnDestroy()
    {
        if (playerHealth != null)
        {
            playerHealth.OnHealthChanged -= UpdateHealth;
            playerHealth.OnPlayerDeath -= ShowGameOverScreen;
        }
        if (playerScore != null) playerScore.OnScoreChanged -= UpdateScore;
        if (GameManager.Instance != null) GameManager.Instance.OnChancesChanged -= UpdateChances;
    }
}