using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System.Linq;

public class HealthUI : MonoBehaviour
{
    public UIDocument uiDocument;
    public PlayerHealthSystem player;

    private List<VisualElement> bars = new List<VisualElement>();
    private VisualElement gameOverPanel;

    void Start()
    {
        if (uiDocument == null || player == null)
        {
            Debug.LogError("HealthUI is missing UIDocument or PlayerHealthSystem reference!");
            return;
        }

        var root = uiDocument.rootVisualElement;
        
        // 1. Get references
        var container = root.Q<VisualElement>("healthBars");
        
        // ADD THIS: Replace "gameOverPanelName" with the exact name you gave your Game Over root box in UI Builder!
        gameOverPanel = root.Q<VisualElement>("GameOverPanel"); 
        
        if (gameOverPanel != null)
        {
            // Hide the game over screen at the start of the game
            gameOverPanel.style.display = DisplayStyle.None;
        }

        if (container == null)
        {
            Debug.LogError("Could not find 'healthBars' container in the UI Document!");
            return;
        }

        bars = container.Query<VisualElement>(className: "health-bar").ToList();

        if (bars.Count != player.MaxLives)
        {
            Debug.LogWarning($"Found {bars.Count} health bars in UI Builder, but player max lives is {player.MaxLives}. Make sure they match!");
        }

        // Subscribe to events
        player.OnHealthChanged += UpdateHealth;
        player.OnPlayerDeath += ShowGameOverScreen;

        // Set the initial visual state
        UpdateHealth(player.CurrentLives, player.MaxLives);
    }

    void UpdateHealth(int current, int max)
    {
        int lostLives = max - current;

        for (int i = 0; i < bars.Count; i++)
        {
            if (i < lostLives)
            {
                bars[i].style.display = DisplayStyle.None; 
            }
            else
            {
                bars[i].style.display = DisplayStyle.Flex; 
            }
        }
    }

    private void ShowGameOverScreen()
    {
        if (gameOverPanel != null)
        {
            // Un-hide the game over panel
            gameOverPanel.style.display = DisplayStyle.Flex;
        }
        else
        {
            Debug.Log("Player died, but I couldn't find a game over panel in UI Builder to show!");
        }
    }

    private void OnDestroy()
    {
        if (player != null)
        {
            player.OnHealthChanged -= UpdateHealth;
            player.OnPlayerDeath -= ShowGameOverScreen; // Always unsubscribe
        }
    }
}