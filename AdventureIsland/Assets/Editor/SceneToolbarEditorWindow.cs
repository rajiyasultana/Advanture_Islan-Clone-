using UnityEngine;
using UnityEditor;
using UnityEditor.Overlays;
using UnityEngine.UIElements;
using System.Collections.Generic;

[Overlay(typeof(SceneView), "Prefab Palette Tools", true)]
public class SceneToolbarEditorWindow : Overlay
{
    private List<GameObject> overlayPrefabs = new List<GameObject>();
    private const string SAVE_KEY = "ScenePrefabOverlay_SavedPaths";

    // Visual Elements needed for reference
    private VisualElement container;
    private ScrollView scrollView;

    public override void OnCreated()
    {
        base.OnCreated();
        LoadPrefabs();
    }

    // Creates the modern VisualElement UI layout
    public override VisualElement CreatePanelContent()
    {
        VisualElement root = new VisualElement();

        root.style.flexGrow = 1;
        // 1. Add Prefab Object Field
        IMGUIContainer objectFieldContainer = new IMGUIContainer(() =>
        {
            GameObject newPrefab = (GameObject)EditorGUILayout.ObjectField("", null, typeof(GameObject), false, GUILayout.Width(120));
            if (newPrefab != null && !overlayPrefabs.Contains(newPrefab))
            {
                overlayPrefabs.Add(newPrefab);
                SavePrefabs();
                RefreshPalette(); // Rebuild the wrapped grid when items are added
            }
        });
        root.Add(objectFieldContainer);

        scrollView = new ScrollView(ScrollViewMode.Vertical);
        scrollView.style.flexGrow = 1;
        scrollView.style.marginTop = 5;
        
        
        // 2. Setup the Grid Container with pure Flexbox Wrapping
        container = new VisualElement();
        container.style.flexDirection = FlexDirection.Row;
        container.style.flexWrap = Wrap.Wrap; // This provides dynamic wrapping on resize!
        container.style.marginTop = 5;
        container.style.width = new Length(100, LengthUnit.Percent);

        
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

            // 3. Create Button Element
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

            // 4. Generate & Display Asset Preview
            Image img = new Image();
            Texture2D preview = AssetPreview.GetAssetPreview(prefab);
            if (preview == null) preview = EditorGUIUtility.IconContent("d_GameObject Icon").image as Texture2D;
            img.image = preview;
            img.scaleMode = ScaleMode.ScaleToFit;
            img.style.paddingLeft = img.style.paddingRight = img.style.paddingTop = img.style.paddingBottom = 4;
            img.style.width = new Length(100, LengthUnit.Percent);
            img.style.height = new Length(100, LengthUnit.Percent);
            btn.Add(img);

            // 5. Native Right-Click Context Menu handling
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

            // 6. Handle Native Drag & Drop out of UI Toolkit
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