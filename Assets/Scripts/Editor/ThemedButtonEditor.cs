using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ThemedButton))]
public class ThemedButtonEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("Apply Theme Now"))
        {
            ((ThemedButton)target).ApplyTheme();
        }
    }
}