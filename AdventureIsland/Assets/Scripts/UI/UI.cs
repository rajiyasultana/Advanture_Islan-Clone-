using UnityEngine;
using UnityEngine.UIElements;

public class UI : MonoBehaviour
{
    public GameManager gameManager;
    [SerializeField] private VideoDisplaySettingsUI videoDisplaySettingsUI;

    private VisualElement _settingsPanel;
    private Button _settingsButton;

    private void Awake()
    {
        FindVideoSettingsUI();
    }

    private void Start()
    {
        FindVideoSettingsUI();
    }

    private void FindVideoSettingsUI()
    {
        if (videoDisplaySettingsUI == null)
        {
            videoDisplaySettingsUI = GetComponent<VideoDisplaySettingsUI>();
            if (videoDisplaySettingsUI == null)
            {
                videoDisplaySettingsUI = FindAnyObjectByType<VideoDisplaySettingsUI>(FindObjectsInactive.Include);
            }
        }
    }

    private void OnEnable()
    {
        var uiDoc = GetComponent<UIDocument>();
        if (uiDoc == null) return;

        VisualElement root = uiDoc.rootVisualElement;
        if (root == null) return;

        FindVideoSettingsUI();

        Button restartButton = root.Q<Button>("restartButton");
        if (restartButton != null && gameManager != null)
        {
            restartButton.clicked -= OnRestartClicked;
            restartButton.clicked += OnRestartClicked;
        }

        _settingsPanel = root.Q<VisualElement>("SettingsPanel");

        // Always wire settingsButton to OpenSettings
        _settingsButton = root.Q<Button>("settingsButton");
        if (_settingsButton != null)
        {
            _settingsButton.clicked -= OpenSettings;
            _settingsButton.clicked += OpenSettings;
        }

        Button closeButton = root.Q<Button>("CloseButton");
        if (closeButton != null)
        {
            closeButton.clicked -= CloseSettings;
            closeButton.clicked += CloseSettings;
        }
    }

    private void OnRestartClicked()
    {
        if (gameManager != null)
        {
            gameManager.ResetGame();
        }
    }

    public void OpenSettings()
    {
        Time.timeScale = 0f;

        FindVideoSettingsUI();

        if (videoDisplaySettingsUI != null)
        {
            videoDisplaySettingsUI.OpenSettings();
        }
        else if (_settingsPanel != null)
        {
            _settingsPanel.style.display = DisplayStyle.Flex;
        }
    }

    public void CloseSettings()
    {
        Time.timeScale = 1f;

        if (videoDisplaySettingsUI != null)
        {
            videoDisplaySettingsUI.CloseSettings();
        }
        else if (_settingsPanel != null)
        {
            _settingsPanel.style.display = DisplayStyle.None;
        }
    }
}
