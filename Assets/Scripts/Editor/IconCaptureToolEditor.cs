using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(IconCaptureTool))]
public class IconCaptureToolEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        IconCaptureTool tool = (IconCaptureTool)target;

        GUILayout.Space(10);

        if (GUILayout.Button("Capture Icon as PNG"))
        {
            tool.CaptureIcon();
        }
    }
}
