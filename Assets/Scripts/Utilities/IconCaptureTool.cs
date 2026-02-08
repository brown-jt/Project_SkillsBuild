using UnityEngine;
using UnityEditor;
using System.IO;

public class IconCaptureTool
{
    [MenuItem("Tools/Capture Icon %#i")]
    public static void CaptureIcon()
    {
        GameObject camObj = GameObject.Find("Camera_Icon");
        if (!camObj)
        {
            Debug.LogError("Camera_Icon not found in scene.");
            return;
        }

        Camera cam = camObj.GetComponent<Camera>();
        if (!cam)
        {
            Debug.LogError("Camera_Icon has no Camera component.");
            return;
        }

        if (cam.targetTexture == null)
        {
            Debug.LogError("Camera_Icon has no RenderTexture assigned.");
            return;
        }

        RenderTexture rt = cam.targetTexture;
        Texture2D tex = new Texture2D(rt.width, rt.height, TextureFormat.RGBA32, false);

        RenderTexture.active = rt;
        cam.Render();
        tex.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        tex.Apply();
        RenderTexture.active = null;

        byte[] png = tex.EncodeToPNG();
        Object.DestroyImmediate(tex);

        string path = "Assets/Icons";
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);

        string filename = $"{path}/icon_{System.DateTime.Now:HHmmss}.png";
        File.WriteAllBytes(filename, png);

        AssetDatabase.Refresh();
        Debug.Log($"Saved icon to: {filename}");
    }
}
