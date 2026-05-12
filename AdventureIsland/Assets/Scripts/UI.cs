using UnityEngine;
using UnityEngine.UIElements;

public class UI : MonoBehaviour
{
    public GameManager gameManager;

    private void OnEnable()
    {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;

        Button restartButton = root.Q<Button>("restartButton");

        restartButton.clicked += () => gameManager.ResetGame();
    }
}
