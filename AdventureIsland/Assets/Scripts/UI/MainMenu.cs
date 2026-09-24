using System;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using Studio23.SS2.SceneLoadingSystem.Data;

public class MainMenu : MonoBehaviour
{
    private UIDocument _uiDocument;
    private Button _playButton;
    private Button _quitButton;

    [SerializeField] private SceneLoadingOperation sceneLoadingOperation; // from the package
    
    private void Awake()
    {
        _uiDocument = GetComponent<UIDocument>();
        
        var root = _uiDocument.rootVisualElement;
        
        // Query buttons from UXML
        _playButton = root.Q<Button>("play-button");
        _quitButton = root.Q<Button>("quit-button");

        // Wire buttons
        _playButton.clicked += StartGame;
        _quitButton.clicked += ExitGame;
    }

    private void OnDestroy()
    {
        _playButton.clicked -= StartGame;
        _quitButton.clicked -= ExitGame;
    }

    private async void StartGame()
    {
        await sceneLoadingOperation.DoSceneOperation();
    }

    private void ExitGame()
    {
        // Exit the application
        Application.Quit();
    }
    
}
