using Studio23.SS2.SceneLoadingSystem.UI; 
using UnityEngine; 
using UnityEngine.UIElements;

public class HorizontalProgressUIControllerT : AbstractLoadingScreenUI
{
    [Header("UI")] 
    [SerializeField] private UIDocument _uiDocument; 
    private VisualElement _loadingImageSlot;

    public override void Initialize()
    {
        VisualElement root = _uiDocument.rootVisualElement; 
        _loadingImageSlot = root.Q<VisualElement>("loading-progress"); 
        base.Initialize();
    }

    public override void UpdateProgress(float progress)
    {
        _loadingImageSlot.style.width = Length.Percent(progress * 100f);
    }
}
