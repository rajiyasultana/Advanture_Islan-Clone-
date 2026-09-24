using Studio23.SS2.SceneLoadingSystem.UI;
using UnityEngine;
using UnityEngine.UIElements;

public class LoopingProgressUiControllerT : AbstractLoadingScreenUI
{
    private VisualElement _loadingImageSlot; 
    [SerializeField] private float _rotationSpeed; 
    [SerializeField] private UIDocument _uiDocument; 
    private float _rotation;

    public override void Initialize()
    {
        VisualElement root = _uiDocument.rootVisualElement; 
        _loadingImageSlot = root.Q<VisualElement>("loading-progress"); 
        base.Initialize();
    }

    public override void UpdateProgress(float progress)
    {
        return;
    }

    private void Update()
    {
        _rotation += _rotationSpeed * Time.deltaTime; 
        _loadingImageSlot.style.rotate = new Rotate( new Angle(_rotation, AngleUnit.Degree) );
    }

}
