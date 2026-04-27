using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System.Linq;

public class HealthUI : MonoBehaviour
{
    [Header("UI Setup")]
    public UIDocument uiDocument; // Drag your single GameUI document here

    [Header("Player References")]
    public PlayerHealthSystem playerHealth;
    public ScoreSystem playerScore;

    private List<VisualElement> bars = new List<VisualElement>();
    
    // UI Elements
    private VisualElement gameOverPanel;
    private VisualElement gameUI;
    private Label scoreLabel;

    void Start()
    {
        if (uiDocument == null || playerHealth == null)
        {
            Debug.LogError("HealthUI is missing UIDocument or PlayerHealthSystem reference!");
            return;
        }

        var root = uiDocument.rootVisualElement;

        // 1. Get the Main Wrapper Elements based on your UXML
        gameUI = root.Q<VisualElement>("GameUI");
        gameOverPanel = root.Q<VisualElement>("GameOverPanel");
        
        // Hide game over screen at start
        if (gameOverPanel != null)
        {
            gameOverPanel.style.display = DisplayStyle.None; 
        }

        // 2. Setup Health Bars
        var container = root.Q<VisualElement>("healthBars");
        if (container != null)
        {
            bars = container.Query<VisualElement>(className: "health-bar").ToList();
        }
        else
        {
            Debug.LogError("Could not find 'healthBars' inside the UI Document!");
        }

        // 3. Setup Score Label
        // Because your UXML has nested score-text classes, we specifically target the actual Label element by type
        scoreLabel = root.Q<Label>(className: "score-text");

        // Subscribe to Player Health Events
        playerHealth.OnHealthChanged += UpdateHealth;
        playerHealth.OnPlayerDeath += ShowGameOverScreen;
        UpdateHealth(playerHealth.CurrentLives, playerHealth.MaxLives);

        // Subscribe to Player Score Events
        if (playerScore != null)
        {
            playerScore.OnScoreChanged += UpdateScore;
            UpdateScore(playerScore.Score); // Initialize at 0
        }
    }

    void UpdateHealth(int current, int max)
    {
        int lostLives = max - current;

        for (int i = 0; i < bars.Count; i++)
        {
            if (i < lostLives)
                bars[i].style.display = DisplayStyle.None; 
            else
                bars[i].style.display = DisplayStyle.Flex; 
        }
    }

    void UpdateScore(int currentScore)
    {
        if (scoreLabel != null)
        {
            // Turns "100" into "000100"
            scoreLabel.text = currentScore.ToString("D6"); 
        }
    }

    private void ShowGameOverScreen()
    {
        if (gameOverPanel != null)
        {
            // Show the Game Over panel
            gameOverPanel.style.display = DisplayStyle.Flex;
            
            // Optional: You can hide the GameUI (HUD) here if you want it to disappear when you die!
            if (gameUI != null)
            {
                gameUI.style.display = DisplayStyle.None;
            }
        }
    }

    private void OnDestroy()
    {
        if (playerHealth != null)
        {
            playerHealth.OnHealthChanged -= UpdateHealth;
            playerHealth.OnPlayerDeath -= ShowGameOverScreen;
        }

        if (playerScore != null)
        {
            playerScore.OnScoreChanged -= UpdateScore;
        }
    }
}