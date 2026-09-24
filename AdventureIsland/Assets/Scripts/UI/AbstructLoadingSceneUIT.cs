using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Studio23.SS2.SceneLoadingSystem.Core;
using Studio23.SS2.SceneLoadingSystem.Data;
using Studio23.SS2.SceneLoadingSystem.Extension;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.UIElements;

public abstract class AbstractLoadingScreenUI : MonoBehaviour
    {
        [Header("Data")]
        [SerializeField]
        public List<Sprite> _backgroundImages;
        [SerializeField] private TextStyleSettings _hintStyle;
        [SerializeField] private LoadingScreenTextTable _hintTable;

        [Header("UI")]
        [SerializeField] private UIDocument _uiDocument;

        private VisualElement _backgroundImageSlot;
        private Label _hintHeaderText;
        private Label _hintDescriptionText;
        private Label _pressAnyKeyText;

        [Header("Config")]
        [SerializeField] private float _hintShowDuration;
        [SerializeField] public float _backgroundFadeDuration;
        [SerializeField] private bool _needKeyPress;

        private CancellationTokenSource _hintCancelTokenSource;
        private CancellationTokenSource _backgroundImageCancelTokenSource;
        private IDisposable _onKeyPressEvent;
        public UnityEvent OnValidAnyKeyPressEvent;

        public virtual void Initialize()
        {
            _hintCancelTokenSource = new CancellationTokenSource();
            _backgroundImageCancelTokenSource = new CancellationTokenSource();

            VisualElement root = _uiDocument.rootVisualElement;

            _backgroundImageSlot = root.Q<VisualElement>("background");
            _hintHeaderText = root.Q<Label>("hint-header");
            _hintDescriptionText = root.Q<Label>("hint-description");
            _pressAnyKeyText = root.Q<Label>("press-any-key");

            if (_backgroundImages != null && _backgroundImages.Count > 0)
            {
                _backgroundImages = ShuffleListExtension.Shuffle(_backgroundImages);
                _backgroundImageSlot.style.backgroundImage =
                    new StyleBackground(_backgroundImages[0]);
            }

            ShowHint();
            CrossFadeBackGroundImages();
        }


        public void OnLoadingDone()
        {
            _hintCancelTokenSource.Cancel();
            _backgroundImageCancelTokenSource.Cancel();

            _hintHeaderText.text = string.Empty;
            _hintDescriptionText.text = string.Empty;

            _pressAnyKeyText.style.display =
                _needKeyPress ? DisplayStyle.Flex : DisplayStyle.None;

            CheckIfKeyPressRequired();
        }

        protected void CheckIfKeyPressRequired()
        {
            if(!_needKeyPress) OnAnyKeyPress();
            else _onKeyPressEvent = InputSystem.onAnyButtonPress.CallOnce(ctrl => OnAnyKeyPress());
        }

        protected void OnAnyKeyPress()
        {
            _onKeyPressEvent?.Dispose();
            Debug.Log("Any Key Pressed");
            OnValidAnyKeyPressEvent?.Invoke();

        }

        public void RemoveLoadingScreen()
        {
            Destroy(gameObject, .1f);

        }

        public virtual void UpdateProgress(float progress){}

        protected async void ShowHint()
        {

            while (!_hintCancelTokenSource.IsCancellationRequested)
            {
                var textData = _hintTable.GetHint();
                _hintHeaderText.text = textData.Title;
                _hintDescriptionText.text = textData.Description;
                await UniTask.Delay(TimeSpan.FromSeconds(_hintShowDuration),cancellationToken: _hintCancelTokenSource.Token,
                    cancelImmediately: true).SuppressCancellationThrow();
            }
        }

        


        protected async void CrossFadeBackGroundImages()
        {
            if (_backgroundImages == null || _backgroundImages.Count < 2)
                return;

            float timer = 0f;

            _backgroundImages = ShuffleListExtension.Shuffle(_backgroundImages);

            Sprite nextImage = _backgroundImages[0];

            while (!_backgroundImageCancelTokenSource.IsCancellationRequested)
            {
                timer += Time.deltaTime;

                float ratio = Mathf.Clamp01(timer / _backgroundFadeDuration);

                _backgroundImageSlot.style.opacity = 1f - ratio;

                if (timer >= _backgroundFadeDuration)
                {
                    timer = 0f;

                    _backgroundImageSlot.style.backgroundImage =
                        new StyleBackground(nextImage);

                    _backgroundImageSlot.style.opacity = 1f;

                    _backgroundImages =
                        ShuffleListExtension.Shuffle(_backgroundImages);

                    nextImage = _backgroundImages[0];
                }

                await UniTask.NextFrame(
                    cancellationToken: _backgroundImageCancelTokenSource.Token,
                    cancelImmediately: true
                ).SuppressCancellationThrow();
            }
        }
    }

