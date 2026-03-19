using SQLite;
using System;
using System.Collections.Generic;
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

    // List of quests and items
    private Dictionary<string, QuestData> _questsDictionary;
    private Dictionary<string, ItemData> _itemsDictionary;

    // Internal representation of the inventory table
    public class InventoryRow
    {
        public int slot_index { get; set; }
        public string item_id { get; set; }
        public int amount { get; set; }
    }

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

        // Initialize runtime dictionaries for quests and items
        InitRuntimeDictionaries();
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

    private void InitRuntimeDictionaries()
    {
        _questsDictionary = new Dictionary<string, QuestData>();
        _itemsDictionary = new Dictionary<string, ItemData>();

        // Load item scriptable objects from Resources folder
        ItemData[] allItems = Resources.LoadAll<ItemData>("Items");

        foreach (var item in allItems)
        {
            if (!_itemsDictionary.ContainsKey(item.itemId)) _itemsDictionary[item.itemId] = item;
            else Debug.LogWarning($"Duplicate item ID found: {item.itemId}. Skipping.");
        }

        Debug.Log($"Loaded {_itemsDictionary.Count} items into the items runtime dictionary.");

        // Load quest scriptable objects from Resources folder
        QuestData[] allQuests = Resources.LoadAll<QuestData>("Quests");

        foreach (var item in allQuests) 
        {
            if (!_questsDictionary.ContainsKey(item.questId)) _questsDictionary[item.questId] = item;
            else Debug.LogWarning($"Duplicate quest ID found: {item.questId}. Skipping.");
        }

        Debug.Log($"Loaded {_questsDictionary.Count} quests into the quests runtime dictionary.");

    }

    private void OnDestroy()
    {
        // Close and dispose of the database connection when the object is destroyed
        _db?.Dispose();
    }

    #region Helper functions to access database

    public bool AddToInventory(ItemData item, int quantity = 1)
    {
        if (item == null || quantity <= 0) return false;

        // If stackable try to add to existing stacks first
        if (item.stackable)
        {
            var stacks = _db.Query<InventoryRow>("SELECT * FROM Inventory WHERE item_id = ? ORDER BY slot_index ASC", item.itemId);

            foreach (var stack in stacks)
            {
                if (stack.amount < item.maxStack)
                {
                    int spaceLeft = item.maxStack - stack.amount;
                    int quantityToAdd = Mathf.Min(spaceLeft, quantity);

                    _db.Execute(
                        "UPDATE Inventory SET quantity = quantity + ? WHERE slot_index = ?", 
                        quantityToAdd, stack.slot_index
                    );

                    quantity -= quantityToAdd;

                    if (quantity <= 0)
                    {
                        return true;
                    }
                }
            }
        }

        // If not stackable or still have quantity left, add to new slot
        while (quantity > 0)
        {
            int quantityToAdd = item.stackable ? Mathf.Min(item.maxStack, quantity) : 1;

            _db.Execute(
                "INSERT INTO Inventory (item_id, quantity) VALUES (?, ?)", 
                item.itemId, quantityToAdd
            );

            quantity -= quantityToAdd;

            if (quantity <= 0)
            {
                return true;
            }
        }

        // If we exit the loop and still have quantity left, something went wrong for now just return false (should never happen with current logic)
        return false;
    }

    public bool RemoveFromInventory(ItemData item, int quantity = 1)
    {
        if (item == null || quantity <= 0) return false;

        var stacks = _db.Query<InventoryRow>(
            "SELECT * FROM Inventory WHERE item_id = ? ORDER BY slot_index ASC",
            item.itemId
        );

        foreach (var stack in stacks)
        {
            if (quantity <= 0) break;

            int toRemove = Mathf.Min(stack.amount, quantity);

            if (stack.amount == toRemove)
            {
                _db.Execute(
                    "DELETE FROM Inventory WHERE slot_index = ?",
                    stack.slot_index
                );
            }
            else
            {
                _db.Execute(
                    "UPDATE Inventory SET quantity = quantity - ? WHERE slot_index = ?",
                    toRemove, stack.slot_index
                );
            }

            quantity -= toRemove;
        }

        return quantity <= 0;
    }

    public Dictionary<ItemData, List<InventoryRow>> LoadInventory()
    {
        var res = new Dictionary<ItemData, List<InventoryRow>>();

        var rows = _db.Query<InventoryRow>("SELECT * FROM Inventory");

        foreach (var row in rows)
        {
            if (_itemsDictionary.TryGetValue(row.item_id, out var itemData))
            {
                if (!res.ContainsKey(itemData)) res[itemData] = new List<InventoryRow>();
                res[itemData].Add(row);
            }
            else
            {
                Debug.LogWarning($"Item ID {row.item_id} in inventory does not exist in items dictionary. Skipping.");
            }
        }

        return res;
    }

    #endregion
}
