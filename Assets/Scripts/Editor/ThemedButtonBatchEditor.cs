using UnityEditor;
using UnityEngine;

public class ThemedButtonBatchEditor : EditorWindow
{
    private UITheme theme;

    [MenuItem("Tools/Apply Theme To All Buttons")]
    public static void ShowWindow()
    {
        GetWindow<ThemedButtonBatchEditor>("Apply UI Theme");
    }

    private void OnGUI()
    {
        GUILayout.Label("Batch Apply UI Theme", EditorStyles.boldLabel);

        theme = (UITheme)EditorGUILayout.ObjectField("Theme", theme, typeof(UITheme), false);

        EditorGUILayout.Space();

        GUI.enabled = theme != null;
        if (GUILayout.Button("Apply Theme To All Buttons", GUILayout.Height(40)))
        {
            ApplyToAll();
        }
        GUI.enabled = true;

        if (theme == null)
            EditorGUILayout.HelpBox("Assign a UITheme asset to continue.", MessageType.Info);
    }

    private void ApplyToAll()
    {
        // Find all ThemedButton components across ALL prefabs and open scenes
        string[] prefabPaths = AssetDatabase.FindAssets("t:Prefab");
        int count = 0;

        // Apply to scene objects
        var sceneButtons = FindObjectsByType<ThemedButton>(FindObjectsSortMode.None);
        foreach (var btn in sceneButtons)
        {
            btn.theme = theme;
            btn.ApplyTheme();
            EditorUtility.SetDirty(btn);
            count++;
        }

        // Apply to prefabs
        foreach (string guid in prefabPaths)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (prefab == null) continue;

            var buttons = prefab.GetComponentsInChildren<ThemedButton>(true);
            if (buttons.Length == 0) continue;

            // Edit prefab properly
            GameObject prefabRoot = PrefabUtility.LoadPrefabContents(path);
            var prefabButtons = prefabRoot.GetComponentsInChildren<ThemedButton>(true);
            foreach (var btn in prefabButtons)
            {
                btn.theme = theme;
                btn.ApplyTheme();
                count++;
            }
            PrefabUtility.SaveAsPrefabAsset(prefabRoot, path);
            PrefabUtility.UnloadPrefabContents(prefabRoot);
        }

        AssetDatabase.SaveAssets();
        Debug.Log($"Applied theme to {count} ThemedButton(s).");
    }
}