using UnityEngine;
using UnityEditor;
using UnityEditor.Overlays;
using System.Collections.Generic;

[Overlay(typeof(SceneView), "Prefab Palette Tools", true)]
public class SceneToolbarEditorWindow : IMGUIOverlay
{
    private List<GameObject> overlayPrefabs = new List<GameObject>();
    private const string SAVE_KEY = "ScenePrefabOverlay_SavedPaths";

    // 1. This runs automatically when the Overlay is initialized or reloaded by Unity
    public override void OnCreated()
    {
        base.OnCreated();
        LoadPrefabs();
    }

    public override void OnGUI()
    {
        // Field to add prefabs dynamically
        GameObject newPrefab = (GameObject)EditorGUILayout.ObjectField("", null, typeof(GameObject), false, GUILayout.Width(120));
        if (newPrefab != null && !overlayPrefabs.Contains(newPrefab))
        {
            overlayPrefabs.Add(newPrefab);
            SavePrefabs(); // Save instantly when a new prefab is added
        }

        if (overlayPrefabs.Count == 0)
        {
            GUILayout.Label("Drop Prefab Here", EditorStyles.miniLabel);
            return;
        }

        // Render items as icon buttons
        GUILayout.BeginHorizontal();
        for (int i = 0; i < overlayPrefabs.Count; i++)
        {
            GameObject prefab = overlayPrefabs[i];
            
            // If an asset was deleted externally, skip it
            if (prefab == null) continue;

            // Generate an automatic asset layout preview icon
            Texture2D preview = AssetPreview.GetAssetPreview(prefab);
            if (preview == null) preview = EditorGUIUtility.IconContent("d_GameObject Icon").image as Texture2D;

            // Display icon button
            Rect buttonRect = GUILayoutUtility.GetRect(32, 32);
            
            // Right-click context menu to remove items from the bar
            if (buttonRect.Contains(Event.current.mousePosition) && Event.current.type == EventType.ContextClick)
            {
                int indexToRemove = i;
                GenericMenu menu = new GenericMenu();
                menu.AddItem(new GUIContent("Remove from toolbar"), false, () => {
                    overlayPrefabs.RemoveAt(indexToRemove);
                    SavePrefabs();
                });
                menu.ShowAsContext();
                Event.current.Use();
            }

            GUI.Box(buttonRect, new GUIContent(preview, $"Drag {prefab.name} into scene\n(Right-click to remove)"), EditorStyles.miniButton);

            // Track mouse drag events right off the overlay icon box
            HandleOverlayDrag(buttonRect, prefab);
        }
        GUILayout.EndHorizontal();
    }

    private void HandleOverlayDrag(Rect itemRect, GameObject prefabToDrag)
    {
        Event currentEvent = Event.current;

        if (itemRect.Contains(currentEvent.mousePosition) && currentEvent.type == EventType.MouseDrag)
        {
            DragAndDrop.PrepareStartDrag();
            DragAndDrop.objectReferences = new Object[] { prefabToDrag };
            DragAndDrop.paths = new string[] { AssetDatabase.GetAssetPath(prefabToDrag) };
            DragAndDrop.StartDrag(prefabToDrag.name);
            currentEvent.Use();
        }
    }

    // 2. Converts GameObjects to project relative paths and saves them to local project registry
    private void SavePrefabs()
    {
        List<string> paths = new List<string>();
        foreach (var prefab in overlayPrefabs)
        {
            if (prefab != null)
            {
                string path = AssetDatabase.GetAssetPath(prefab);
                if (!string.IsNullOrEmpty(path))
                {
                    paths.Add(path);
                }
            }
        }
        // Join paths together with a semicolon separator (e.g. "Assets/P1.prefab;Assets/P2.prefab")
        string combinedPaths = string.Join(";", paths);
        EditorPrefs.SetString(SAVE_KEY, combinedPaths);
    }

    // 3. Reads the saved paths string and loads the actual assets back into memory
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
                    if (loadedPrefab != null)
                    {
                        overlayPrefabs.Add(loadedPrefab);
                    }
                }
            }
        }
    }
}
