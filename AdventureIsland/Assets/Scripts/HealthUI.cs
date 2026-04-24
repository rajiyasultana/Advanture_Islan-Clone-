using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System.Linq;

public class HealthUI : MonoBehaviour
{
    public UIDocument uiDocument;
    public PlayerHealthSystem player;

    private List<VisualElement> bars = new List<VisualElement>();

    void Start()
    {
        if (uiDocument == null || player == null)
        {
            Debug.LogError("HealthUI is missing UIDocument or PlayerHealthSystem reference!");
            return;
        }

        var root = uiDocument.rootVisualElement;
        var container = root.Q<VisualElement>("healthBars");

        if (container == null)
        {
            Debug.LogError("Could not find 'healthBars' container in the UI Document!");
            return;
        }

        // Find all visual elements inside the container that have the "health-bar" CSS class
        bars = container.Query<VisualElement>(className: "health-bar").ToList();

        if (bars.Count != player.MaxLives)
        {
            Debug.LogWarning($"Found {bars.Count} health bars in UI Builder, but player max lives is {player.MaxLives}. Make sure they match!");
        }

        // Subscribe to the health changed event
        player.OnHealthChanged += UpdateHealth;

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

    private void OnDestroy()
    {
        // Always unsubscribe from events when the object is destroyed to prevent memory leaks
        if (player != null)
        {
            player.OnHealthChanged -= UpdateHealth;
        }
    }
}