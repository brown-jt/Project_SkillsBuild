using System.IO;
using UnityEditor;
using UnityEngine;

public class IconCaptureTool : MonoBehaviour
{
    [Header("References")]
    public Camera iconCamera;
    public Transform itemRoot; // The object holding the item to capture

    [Header("Output")]
    public string outputFolder = "Assets/Icons";

    public void CaptureIcon()
    {
        if (iconCamera == null)
        {
            Debug.LogError("Icon Camera is not assigned.");
            return;
        }

        if (iconCamera.targetTexture == null)
        {
            Debug.LogError("Icon Camera has no RenderTexture assigned.");
            return;
        }

        if (itemRoot == null || itemRoot.childCount == 0)
        {
            Debug.LogError("Item Root has no item to capture.");
            return;
        }

        RenderTexture rt = iconCamera.targetTexture;
        Texture2D tex = new Texture2D(rt.width, rt.height, TextureFormat.RGBA32, false);

        RenderTexture.active = rt;
        iconCamera.Render();
        tex.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        tex.Apply();
        RenderTexture.active = null;

        byte[] png = tex.EncodeToPNG();
        DestroyImmediate(tex);

        if (!Directory.Exists(outputFolder))
            Directory.CreateDirectory(outputFolder);

        // Use the item GameObject name for the file
        string itemName = itemRoot.GetChild(0).name;
        string safeName = MakeSafeFilename(itemName);

        string filename = $"{outputFolder}/Icon_{safeName}.png";
        File.WriteAllBytes(filename, png);

#if UNITY_EDITOR
        // Refresh to see the newly created file
        UnityEditor.AssetDatabase.Refresh();

        // Get the importer for the new texture
        string assetPath = filename.Replace(Application.dataPath, "Assets");
        var importer = UnityEditor.AssetImporter.GetAtPath(assetPath) as UnityEditor.TextureImporter;
        if (importer != null)
        {
            importer.textureType = UnityEditor.TextureImporterType.Sprite;
            importer.spriteImportMode = UnityEditor.SpriteImportMode.Single;
            importer.alphaIsTransparency = true;
            importer.SaveAndReimport();
        }
#endif

        Debug.Log($"Saved icon: {filename}");
    }

    private string MakeSafeFilename(string name)
    {
        foreach (char c in Path.GetInvalidFileNameChars())
        {
            name = name.Replace(c, '_');
        }
        return name;
    }

#if UNITY_EDITOR
    // Optional hotkey: Ctrl + Shift + I
    [MenuItem("Tools/Capture Icon %#i")]
    private static void CaptureIconHotkey()
    {
        // Find the IconRig in the scene
        IconCaptureTool tool = FindFirstObjectByType<IconCaptureTool>();
        if (tool != null)
        {
            tool.CaptureIcon();
        }
        else
        {
            Debug.LogWarning("No IconCaptureTool found in the scene.");
        }
    }
#endif
}
