using SQLite;
using System.IO;
using System.Linq;
using UnityEngine;

public class DatabaseManager : MonoBehaviour
{
    // Singleton instance
    public static DatabaseManager Instance { get; private set; }

    private readonly string _dbName = "project_skillsbuild_save_data.db";
    private string _dbPath;
    private SQLiteConnection _db;
    private void Awake()
    {
        // Ensure singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Getting the path to the database file
        _dbPath = Path.Combine(Application.persistentDataPath, _dbName);

        // Check to see if database already exists (if path exists)
        bool isNewDatabase = !File.Exists(_dbPath);

        // Create a new database connection
        // This will also create the database file if it doesn't exist
        _db = new SQLiteConnection(_dbPath);

        // Ensure the database has foreign keys enabled
        _db.Execute("PRAGMA foreign_keys = ON;");

        if (isNewDatabase)
        {
            // If the database is new, we need to set it up
            Debug.LogWarning("No database found.. Setting up new database");
            SetupDatabase();
        }
        else
        {
            Debug.Log("Database already exists. No setup needed.");
        }
    }

    /// <summary>
    /// Public getter for the database connection
    /// </summary>
    public SQLiteConnection DB => _db;

    /// <summary>
    /// Executes the SQL commands to set up the database schema
    /// </summary>
    private void SetupDatabase()
    {
        // Create tables if they don't exist
        string setupPath = Path.Combine(Application.streamingAssetsPath, "setup_database.sql");
        string setupSQL = File.ReadAllText(setupPath);

        // Execute the SQL commands to set up the database
        try 
        {
            // Split commands by semicolon
            // Trim whitespace on each command
            // Filter out any empty commands
            var commands = setupSQL
                .Split(';')
                .Select(cmd => cmd.Trim())          
                .Where(cmd => !string.IsNullOrEmpty(cmd))
                .ToList();

            foreach (var cmd in commands)
            {
                _db.Execute(cmd);
            }

            Debug.Log("Successfully finished setting up new database");
        }
        catch (SQLiteException ex)
        {
            Debug.LogError($"Error setting up database: {ex.Message}");
        }
    }

    private void OnDestroy()
    {
        // Close and dispose of the database connection when the object is destroyed
        _db?.Dispose();
    }
}
