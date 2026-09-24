using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Studio23.SS2.Settings.Video.Core;

[RequireComponent(typeof(UIDocument))]
public class VideoDisplaySettingsUI : MonoBehaviour
{
    private UIDocument _uiDocument;
    private VisualElement _settingsPanel;
    private Button _settingsButton;
    private DropdownField _resolutionDropdown;
    private DropdownField _displayModeDropdown;
    private DropdownField _vSyncDropdown;
    private Slider _brightnessSlider;
    private Slider _renderScaleSlider;
    private Button _applyButton;
    private Button _closeButton;

    [SerializeField] private DisplayController _displayController;
    private bool _isSetupDone = false;

    private void Awake()
    {
        
        _uiDocument = GetComponent<UIDocument>();
    }

    private void Start()
    {
        // Settings window should be hidden at the start of the game
        CloseSettings();
    }

    private void OnEnable()
    {
        InitializeElements();
    }

    private void InitializeElements()
    {
        if (_uiDocument == null) return;

        var root = _uiDocument.rootVisualElement;
        if (root == null) return;

        // Query visual elements from UXML
        _settingsPanel = root.Q<VisualElement>("SettingsPanel") ?? root.Q<VisualElement>("DisplaySettingsBackdrop");
        _settingsButton = root.Q<Button>("settingsButton");
        _resolutionDropdown = root.Q<DropdownField>("ResolutionDropdown");
        _displayModeDropdown = root.Q<DropdownField>("DisplayModeDropdown");
        _vSyncDropdown = root.Q<DropdownField>("VSyncDropdown");
        _brightnessSlider = root.Q<Slider>("BrightnessSlider");
        _renderScaleSlider = root.Q<Slider>("RenderScaleSlider");
        _applyButton = root.Q<Button>("ApplyButton");
        _closeButton = root.Q<Button>("CloseButton");

        // Wire buttons
        if (_settingsButton != null)
        {
            _settingsButton.clicked -= OpenSettings;
            _settingsButton.clicked += OpenSettings;
        }

        if (_closeButton != null)
        {
            _closeButton.clicked -= CloseSettings;
            _closeButton.clicked += CloseSettings;
        }

        if (_applyButton != null)
        {
            _applyButton.clicked -= CloseSettings;
            _applyButton.clicked += CloseSettings;
        }

        // Retrieve display controller if not manually assigned in inspector
        if (_displayController == null && VideoSettingsManager.Instance != null)
        {
            _displayController = VideoSettingsManager.Instance.DisplayController;
        }

        if (_displayController != null)
        {
            SetupUI();
        }
    }

    public void OpenSettings()
    {
        // Pause the game
        Time.timeScale = 0f;

        // Ensure display controller is connected if initialized late
        if (_displayController == null && VideoSettingsManager.Instance != null)
        {
            _displayController = VideoSettingsManager.Instance.DisplayController;
            SetupUI();
        }

        if (_uiDocument != null && _uiDocument.rootVisualElement != null)
        {
            _uiDocument.rootVisualElement.style.display = DisplayStyle.Flex;
        }

        if (_settingsPanel != null)
        {
            _settingsPanel.style.display = DisplayStyle.Flex;
        }
    }

    public void CloseSettings()
    {
        // Resume the game
        Time.timeScale = 1f;

        if (_settingsPanel != null)
        {
            _settingsPanel.style.display = DisplayStyle.None;
        }

        // If this script is on a dedicated Settings GameObject, hide its entire root document
        if (_uiDocument != null && _uiDocument.rootVisualElement != null && _uiDocument.rootVisualElement.Q<VisualElement>("GameUI") == null)
        {
            _uiDocument.rootVisualElement.style.display = DisplayStyle.None;
        }
    }

    private void SetupUI()
    {
        if (_isSetupDone || _displayController == null) return;
        _isSetupDone = true;

        // 1. Resolutions
        if (_resolutionDropdown != null)
        {
            var resolutions = _displayController.GetSupportedResolutions();
            List<string> resolutionChoices = new List<string>();
            for (int i = 0; i < resolutions.Length; i++)
            {
                resolutionChoices.Add(resolutions[i].ToString());
            }
            _resolutionDropdown.choices = resolutionChoices;
            int currentResIndex = _displayController.GetSelectedResolutionIndex();
            if (currentResIndex >= 0 && currentResIndex < resolutionChoices.Count)
            {
                _resolutionDropdown.index = currentResIndex;
            }
            _resolutionDropdown.RegisterValueChangedCallback(evt =>
            {
                int index = _resolutionDropdown.index;
                if (index >= 0)
                {
                    _displayController.ChangeResolution(index);
                }
            });
        }

        // 2. FullScreen Modes
        if (_displayModeDropdown != null)
        {
            var screenModes = _displayController.GetFullScreenModes();
            List<string> modeChoices = new List<string>();
            int selectedModeIndex = 0;
            for (int i = 0; i < screenModes.Length; i++)
            {
                modeChoices.Add(screenModes[i].ToString());
                if (screenModes[i] == Screen.fullScreenMode)
                {
                    selectedModeIndex = i;
                }
            }
            _displayModeDropdown.choices = modeChoices;
            _displayModeDropdown.index = selectedModeIndex;
            _displayModeDropdown.RegisterValueChangedCallback(evt =>
            {
                int index = _displayModeDropdown.index;
                if (index >= 0)
                {
                    _displayController.ChangeFullScreenMode(index);
                }
            });
        }

        // 3. V-Sync
        if (_vSyncDropdown != null)
        {
            List<string> vsyncChoices = new List<string>() { "Off", "On (60 FPS)", "Every 2nd V-Blank (30 FPS)" };
            _vSyncDropdown.choices = vsyncChoices;
            _vSyncDropdown.index = Mathf.Clamp(QualitySettings.vSyncCount, 0, 2);
            _vSyncDropdown.RegisterValueChangedCallback(evt =>
            {
                int index = _vSyncDropdown.index;
                if (index >= 0)
                {
                    _displayController.ChangeVSync(index);
                }
            });
        }

        // 4. Brightness
        if (_brightnessSlider != null)
        {
            _brightnessSlider.RegisterValueChangedCallback(evt =>
            {
                _displayController.SetBrightness(evt.newValue);
            });
        }

        // 5. Render Scale
        if (_renderScaleSlider != null)
        {
            _renderScaleSlider.RegisterValueChangedCallback(evt =>
            {
                _displayController.SetRenderScale(evt.newValue);
            });
        }
    }
}
