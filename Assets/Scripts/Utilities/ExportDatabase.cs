using System.IO;
using UnityEngine;

public class ExportDatabase : MonoBehaviour
{
    public void ExportDatabaseToDownloads()
    {
        string srcPath = DatabaseManager.Instance.GetDBPath();

        string downloadsPath = Path.Combine(
            System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile),
            "Downloads"
        );

        string destinationPath = Path.Combine(downloadsPath, Path.GetFileName(srcPath));

        try
        {
            File.Copy(srcPath, destinationPath, true);
            Debug.Log($"Database exported successfully to: {destinationPath}");

            // Opening the folder to check if the file is there
            System.Diagnostics.Process.Start("explorer.exe", downloadsPath);
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Failed to export database: {ex.Message}");
        }
    }
}
