using UnityEngine;
using UnityEditor;
using UnityEditor.Overlays;
using UnityEngine.UIElements;
using UnityEditor.UIElements; // Required for modern ObjectField
using System.Collections.Generic;

[Overlay(typeof(SceneView), "Prefab Palette Tools", true)]
public class SceneToolbarEditorWindow : Overlay
{
    private List<GameObject> overlayPrefabs = new List<GameObject>();
    private const string SAVE_KEY = "ScenePrefabOverlay_SavedPaths";

    // Visual Elements needed for reference
    private VisualElement container;
    private ScrollView scrollView;

    // Keeps track of elements whose previews are still loading from disk
    private Dictionary<Image, GameObject> pendingPreviews = new Dictionary<Image, GameObject>();

    public override void OnCreated()
    {
        base.OnCreated();
        LoadPrefabs();
        
        // Listen to global editor updates to catch asset previews once they finish caching
        EditorApplication.update += MonitorPendingPreviews;
    }

    // Always clean up event delegates when the overlay is destroyed to avoid memory leaks
    public override void OnWillBeDestroyed()
    {
        base.OnWillBeDestroyed();
        EditorApplication.update -= MonitorPendingPreviews;
    }

    // Creates the modern VisualElement UI layout
    public override VisualElement CreatePanelContent()
    {
        VisualElement root = new VisualElement();
        root.style.flexGrow = 1;

        // FIX 1: Modern Native ObjectField completely replaces the glitchy IMGUI version
        ObjectField objectField = new ObjectField("")
        {
            objectType = typeof(GameObject),
            allowSceneObjects = false
        };
        objectField.style.width = 120;
        objectField.style.marginBottom = 5;

        // Registers a modern change-callback tracking completion state safely
        objectField.RegisterValueChangedCallback(evt =>
        {
            GameObject newPrefab = evt.newValue as GameObject;
            if (newPrefab != null)
            {
                if (!overlayPrefabs.Contains(newPrefab))
                {
                    overlayPrefabs.Add(newPrefab);
                    SavePrefabs();
                    RefreshPalette();
                }
                // Instantly clear the picker slot so it's fresh for the next input
                objectField.SetValueWithoutNotify(null);
            }
        });
        root.Add(objectField);

        // Grid Container Scrolling Constraints Setup
        scrollView = new ScrollView(ScrollViewMode.Vertical);
        scrollView.style.flexGrow = 1;
        scrollView.style.marginTop = 5;
        
        // Force structural wrapping constraints directly onto the viewport content container
        scrollView.contentContainer.style.flexDirection = FlexDirection.Row;
        scrollView.contentContainer.style.flexWrap = Wrap.Wrap;
        scrollView.contentContainer.style.width = new StyleLength(StyleKeyword.Auto);
        
        container = new VisualElement();
        container.style.flexDirection = FlexDirection.Row;
        container.style.flexWrap = Wrap.Wrap; 
        container.style.flexGrow = 1;

        scrollView.Add(container);
        root.Add(scrollView);

        // Populate items initially
        RefreshPalette();

        return root;
    }

    private void RefreshPalette()
    {
        if (container == null) return;
        container.Clear();
        pendingPreviews.Clear();

        if (overlayPrefabs.Count == 0)
        {
            Label emptyLabel = new Label("Drop Prefab Here");
            emptyLabel.style.unityFontStyleAndWeight = FontStyle.Italic;
            emptyLabel.style.color = new Color(0.6f, 0.6f, 0.6f);
            container.Add(emptyLabel);
            return;
        }

        const float iconSize = 56f;

        for (int i = 0; i < overlayPrefabs.Count; i++)
        {
            GameObject prefab = overlayPrefabs[i];
            if (prefab == null) continue;

            int index = i; // Cache index for closure scopes

            // Create Button Element
            VisualElement btn = new VisualElement();
            btn.style.width = iconSize;
            btn.style.height = iconSize;
            btn.style.marginLeft = 2;
            btn.style.marginRight = 2;
            btn.style.marginTop = 2;
            btn.style.marginBottom = 2;
            btn.tooltip = $"Drag {prefab.name} into scene\n(Right-click to remove)";

            // Style like standard Unity UI Button
            btn.style.backgroundColor = new Color(0.22f, 0.22f, 0.22f);
            btn.style.borderTopWidth = 1; btn.style.borderBottomWidth = 1;
            btn.style.borderLeftWidth = 1; btn.style.borderRightWidth = 1;
            btn.style.borderTopColor = btn.style.borderBottomColor = btn.style.borderLeftColor = btn.style.borderRightColor = new Color(0.15f, 0.15f, 0.15f);

            // Generate & Display Asset Preview
            Image img = new Image();
            Texture2D preview = AssetPreview.GetAssetPreview(prefab);
            
            if (preview == null)
            {
                // FIX 2: If preview isn't ready post-playmode, show a fallback and queue it up for update monitoring
                preview = EditorGUIUtility.IconContent("d_GameObject Icon").image as Texture2D;
                if (!pendingPreviews.ContainsKey(img))
                {
                    pendingPreviews.Add(img, prefab);
                }
            }
            
            img.image = preview;
            img.scaleMode = ScaleMode.ScaleToFit;
            img.style.paddingLeft = img.style.paddingRight = img.style.paddingTop = img.style.paddingBottom = 4;
            img.style.width = new Length(100, LengthUnit.Percent);
            img.style.height = new Length(100, LengthUnit.Percent);
            btn.Add(img);

            // Native Right-Click Context Menu handling
            btn.RegisterCallback<PointerDownEvent>(evt =>
            {
                if (evt.button == 1) // Right Click
                {
                    GenericMenu menu = new GenericMenu();
                    menu.AddItem(new GUIContent("Remove from toolbar"), false, () =>
                    {
                        overlayPrefabs.RemoveAt(index);
                        SavePrefabs();
                        RefreshPalette();
                    });
                    menu.ShowAsContext();
                    evt.StopPropagation();
                }
            });

            // Handle Native Drag & Drop out of UI Toolkit
            btn.RegisterCallback<PointerMoveEvent>(evt =>
            {
                if (evt.pressedButtons == 1) // Left click dragging
                {
                    DragAndDrop.PrepareStartDrag();
                    DragAndDrop.objectReferences = new Object[] { prefab };
                    DragAndDrop.paths = new string[] { AssetDatabase.GetAssetPath(prefab) };
                    DragAndDrop.StartDrag(prefab.name);
                    evt.StopPropagation();
                }
            });

            container.Add(btn);
        }
    }

    // FIX 2 Engine Loop: Constantly watches for asset icons generated on domain changes
    private void MonitorPendingPreviews()
    {
        if (pendingPreviews.Count == 0) return;

        List<Image> completedImages = new List<Image>();

        foreach (var kvp in pendingPreviews)
        {
            if (kvp.Value == null || kvp.Key == null) continue;

            // Try loading asset preview again
            Texture2D readyPreview = AssetPreview.GetAssetPreview(kvp.Value);
            if (readyPreview != null)
            {
                kvp.Key.image = readyPreview;
                completedImages.Add(kvp.Key);
            }
        }

        // Clean evaluated images out of queue
        foreach (var img in completedImages)
        {
            pendingPreviews.Remove(img);
        }
    }

    // Converts GameObjects to project relative paths and saves them to local project registry
    private void SavePrefabs()
    {
        List<string> paths = new List<string>();
        foreach (var prefab in overlayPrefabs)
        {
            if (prefab != null)
            {
                string path = AssetDatabase.GetAssetPath(prefab);
                if (!string.IsNullOrEmpty(path)) paths.Add(path);
            }
        }
        string combinedPaths = string.Join(";", paths);
        EditorPrefs.SetString(SAVE_KEY, combinedPaths);
    }

    // Reads the saved paths string and loads the actual assets back into memory
    private void LoadPrefabs()
    {
        overlayPrefabs.Clear();
        if (EditorPrefs.HasKey(SAVE_KEY))
        {
            string combinedPaths = EditorPrefs.GetString(SAVE_KEY);
            if (!string.IsNullOrEmpty(combinedPaths))
            {
                string[] paths = combinedPaths.Split(';');
                foreach (string path in paths)
                {
                    GameObject loadedPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                    if (loadedPrefab != null) overlayPrefabs.Add(loadedPrefab);
                }
            }
        }
    }
}
