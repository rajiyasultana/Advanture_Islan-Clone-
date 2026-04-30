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
    private VisualElement transitionPanel;
    private Label transitionScore;
    private Label transitionLive;
    private Label scoreLabel;
    private Label chancesLabel; // <--- Target the '3 lives' label

    void Start()
    {
        if (uiDocument == null || playerHealth == null) return;

        var root = uiDocument.rootVisualElement;

        gameUI = root.Q<VisualElement>("GameUI");
        gameOverPanel = root.Q<VisualElement>("GameOverPanel");
        transitionPanel = root.Q<VisualElement>("TransitionPanel");

        if (gameOverPanel != null) gameOverPanel.style.display = DisplayStyle.None; 
        if(transitionPanel != null) transitionPanel.style.display = DisplayStyle.None;

        var container = root.Q<VisualElement>("healthBars");
        if (container != null) bars = container.Query<VisualElement>(className: "health-bar").ToList();
        
        scoreLabel = root.Q<Label>(className: "score-text");
        chancesLabel = root.Q<Label>(className: "life-text");
        transitionScore = root.Q<Label>("TransitionScore");
        transitionLive = root.Q<Label>("TransitionLives");

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
            GameManager.Instance.OnShowTransitionPanel += ShowTransitionScreen;
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

    private void ShowTransitionScreen(int score, int chancesLeft)
    {
        if (transitionPanel != null)
        {
            transitionPanel.style.display = DisplayStyle.Flex;

            if (transitionScore != null) transitionScore.text = score.ToString("D6");
            if (transitionLive != null) transitionLive.text = chancesLeft.ToString();
            if (gameUI != null) gameUI.style.display = DisplayStyle.None;
        }
    }

    private void OnDestroy()
    {
        if (playerHealth != null)
        {
            playerHealth.OnHealthChanged -= UpdateHealth;
            playerHealth.OnPlayerDeath -= ShowGameOverScreen;
        }
        if (playerScore != null) playerScore.OnScoreChanged -= UpdateScore;
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnChancesChanged -= UpdateChances;
            GameManager.Instance.OnShowTransitionPanel -= ShowTransitionScreen;
        }
    }
}