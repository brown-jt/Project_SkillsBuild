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

    public Dictionary<string, ItemData> ItemsDict => _itemsDictionary;
    public Dictionary<string, QuestData> QuestsDict => _questsDictionary;

    // Internal representation of the inventory table
    [Table("Inventory")]
    public class InventoryRow
    {
        [PrimaryKey]
        public int slot_index { get; set; }
        public string item_id { get; set; }
        public int quantity { get; set; }
    }

    // Internal representation of the quests table
    [Table("Quests")]
    public class QuestRow
    {
        [PrimaryKey]
        public string quest_id { get; set; }
        public int zone_id { get; set; }
        public bool accepted { get; set; }
        public bool completed { get; set; }
    }

    // Internal representation of the quest objectives table
    [Table("QuestObjectives")]
    public class QuestObjectiveRow
    {
        [PrimaryKey, AutoIncrement]
        public int id { get; set; }
        public string quest_id { get; set; }
        public int objective_index { get; set; }
        public int current_amount { get; set; }
        public bool is_complete { get; set; }
    }

    // Internal representation of the question hints table
    [Table("QuestionHints")]
    public class QuestionHintRow
    {
        [PrimaryKey, AutoIncrement]
        public int id { get; set; }
        public string quest_id { get; set; }
        public int question_index { get; set; }
        public string response { get; set; }
        public string created_at { get; set; }
        public string last_used_at { get; set; }
        public int usage_count { get; set; }
    }

    // Max usage of question hint before retired - Keeping for analytical purposes
    private const int HINT_MAX_USAGE = 3;


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

        // Load quest scriptable objects from Resources folder
        QuestData[] allQuests = Resources.LoadAll<QuestData>("Quests");

        foreach (var quest in allQuests) 
        {
            if (!_questsDictionary.ContainsKey(quest.questId))
            {
                _questsDictionary[quest.questId] = quest;
                // For quests we also want to ensure the database is aware of them so we add them to the Quests table if they don't already exist
                try
                {
                    _db.Execute(
                        "INSERT OR IGNORE INTO Quests (quest_id, zone_id) VALUES (?, ?)",
                        quest.questId, (int)quest.zoneId
                    );
                }
                catch (SQLiteException ex)
                {
                    Debug.LogError($"Error inserting quest into database: {ex.Message}");
                }
            }
            else Debug.LogWarning($"Duplicate quest ID found: {quest.questId}. Skipping.");
        }
    }

    private void OnDestroy()
    {
        // Close and dispose of the database connection when the object is destroyed
        _db?.Dispose();
    }

    #region Helper functions to access database

    public bool AddToInventory(ItemData item, int slotIndex, int quantity)
    {
        if (item == null || quantity <= 0) return false;

        // If stackable try to add to existing stacks first
        if (item.stackable)
        {
            var stacks = _db.Query<InventoryRow>("SELECT * FROM Inventory WHERE item_id = ? ORDER BY slot_index ASC", item.itemId);

            foreach (var stack in stacks)
            {
                if (stack.quantity < item.maxStack)
                {
                    int spaceLeft = item.maxStack - stack.quantity;
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
                "INSERT INTO Inventory (slot_index, item_id, quantity) VALUES (?, ?, ?)", 
                slotIndex, item.itemId, quantityToAdd
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

            int toRemove = Mathf.Min(stack.quantity, quantity);

            if (stack.quantity == toRemove)
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

    public void ClearInventorySlot(int slotIndex)
    {
        _db.Execute(
            "DELETE FROM Inventory WHERE slot_index = ?",
            slotIndex
        );
    }

    public void SwapInventorySlots(int slotA, int slotB)
    {
        _db.RunInTransaction(() =>
        {
            var slotAData = _db.Find<InventoryRow>(slotA);
            var slotBData = _db.Find<InventoryRow>(slotB);

            if (slotAData == null && slotBData == null)
            {
                // Both slots are empty, nothing to swap
                return;
            }

            else if (slotAData != null && slotBData != null)
            {
                // Both slots have items, swap their data
                string tempItemId = slotAData.item_id;
                int tempQuantity = slotAData.quantity;

                _db.Execute(
                    "UPDATE Inventory SET item_id = ?, quantity = ? WHERE slot_index = ?",
                    slotBData.item_id, slotBData.quantity, slotAData.slot_index
                );
                _db.Execute(
                    "UPDATE Inventory SET item_id = ?, quantity = ? WHERE slot_index = ?",
                    tempItemId, tempQuantity, slotBData.slot_index
                );
            }

            else if (slotAData != null)
            {
                // Slot A has an item and Slot B is empty, move A to B
                _db.Execute(
                    "UPDATE Inventory SET slot_index = ? WHERE slot_index = ?",
                    slotB, slotA
                );
            }

            else if (slotBData != null)
            {
                // Slot B has an item and Slot A is empty, move B to A
                _db.Execute(
                    "UPDATE Inventory SET slot_index = ? WHERE slot_index = ?",
                    slotA, slotB
                );
            }
        });
    }

    public Dictionary<string, List<InventoryRow>> LoadInventory()
    {
        var res = new Dictionary<string, List<InventoryRow>>();

        var rows = _db.Query<InventoryRow>("SELECT * FROM Inventory");

        foreach (var row in rows)
        {
            if (!res.ContainsKey(row.item_id)) res[row.item_id] = new List<InventoryRow>();
            res[row.item_id].Add(row);
        }

        return res;
    }

    public bool AcceptQuest(QuestData quest)
    {
        if (quest == null) return false;

        try
        {
            _db.Execute(
                "UPDATE Quests SET active = 1 WHERE quest_id = ?",
                quest.questId
            );
            return true;
        }
        catch (SQLiteException ex)
        {
            Debug.LogError($"Error accepting quest: {ex.Message}");
            return false;
        }
    }

    public bool CompleteQuest(QuestData quest)
    {
        if (quest == null) return false;
        try
        {
            _db.Execute(
                "UPDATE Quests SET completed = 1, active = 0 WHERE quest_id = ?",
                quest.questId
            );
            return true;
        }
        catch (SQLiteException ex)
        {
            Debug.LogError($"Error completing quest: {ex.Message}");
            return false;
        }
    }

    public List<QuestData> LoadActiveQuests()
    {
        var res = new List<QuestData>();

        var rows = _db.Query<QuestRow>("SELECT * FROM Quests WHERE active = 1 AND completed = 0");

        foreach (var row in rows)
        {
            if (_questsDictionary.TryGetValue(row.quest_id, out var questInstance))
            {
                if (!res.Contains(questInstance)) res.Add(questInstance);
            }
            else
            {
                Debug.LogWarning($"Quest ID {row.quest_id} in quests table does not exist in runtime quest dictionary. Skipping.");
            }
        }

        return res;
    }

    public List<QuestData> LoadCompletedQuests()
    {
        var res = new List<QuestData>();

        var rows = _db.Query<QuestRow>("SELECT * FROM Quests WHERE completed = 1 AND active = 0");

        foreach (var row in rows)
        {
            if (_questsDictionary.TryGetValue(row.quest_id, out var questInstance))
            {
                if (!res.Contains(questInstance)) res.Add(questInstance);
            }
            else
            {
                Debug.LogWarning($"Quest ID {row.quest_id} in quests table does not exist in runtime quest dictionary. Skipping.");
            }
        }

        return res;
    }

    public bool AddQuestObjectives(QuestInstance questInstance)
    {
        if (questInstance == null || questInstance.questData == null) return false;

        try
        {
            for (var i = 0; i < questInstance.questData.objectives.Count; i++)
            {
                var objective = questInstance.questData.objectives[i];
                _db.Execute(
                    "INSERT INTO Quest_Objectives (quest_id, objective_index, current_amount, is_complete) VALUES (?, ?, ?, ?)",
                    questInstance.questData.questId, i, 0, false
                );
            }
            return true;
        }
        catch (SQLiteException ex)
        {
            Debug.LogError($"Error adding quest objectives: {ex.Message}");
            return false;
        }
    }

    public bool UpdateObjectiveProgress(QuestInstance questInstance, int objectiveIndex, int newAmount, bool isComplete)
    {
        if (questInstance == null || questInstance.questData == null) return false;
        try
        {
            _db.Execute(
                "UPDATE Quest_Objectives SET current_amount = ?, is_complete = ? WHERE quest_id = ? AND objective_index = ?",
                newAmount, isComplete, questInstance.questData.questId, objectiveIndex
            );
            return true;
        }
        catch (SQLiteException ex)
        {
            Debug.LogError($"Error updating quest objective: {ex.Message}");
            return false;
        }
    }

    public List<QuestObjectiveRow> LoadQuestObjectives(string questId)
    {
        try
        {
            var rows = _db.Query<QuestObjectiveRow>(
                "SELECT * FROM Quest_Objectives WHERE quest_id = ? ORDER BY objective_index ASC",
                questId
            );
            return rows;
        }
        catch (SQLiteException ex)
        {
            Debug.LogError($"Error loading quest objectives: {ex.Message}");
            return new List<QuestObjectiveRow>();
        }
    }

    public List<QuestionHintRow> GetAIResponses(string questId, int questionIndex)
    {
        try
        {
            return _db.Query<QuestionHintRow>(
                "SELECT * FROM Question_Hints WHERE quest_id = ? AND question_index = ? AND is_active = 1",
                questId, questionIndex
            );
        }
        catch (SQLiteException ex)
        {
            Debug.LogError($"Error fetching AI responses: {ex.Message}");
            return new List<QuestionHintRow>();
        }
    }

    public void InsertAIResponse(string questId, int questionIndex, string response)
    {
        try
        {
            string currentTime = DateTime.UtcNow.ToString("o");
            _db.Execute(
                "INSERT INTO Question_Hints (quest_id, question_index, response, created_at, usage_count) VALUES (?, ?, ?, ?, 0)",
                questId, questionIndex, response, currentTime
            );
        }
        catch (SQLiteException ex)
        {
            Debug.LogError($"Error inserting AI response: {ex.Message}");
        }
    }

    public void UpdateAIResponseUsage(int id, int currentUsage)
    {
        int newUsage = currentUsage + 1;
        bool isActive = newUsage < HINT_MAX_USAGE;

        try
        {
            string currentTime = DateTime.UtcNow.ToString("o");
            _db.Execute(
                "UPDATE Question_Hints SET usage_count = ?, last_used_at = ?, is_active = ? WHERE id = ?",
                newUsage, currentTime, isActive, id
            );
        }
        catch (SQLiteException ex)
        {
            Debug.LogError($"Error updating usage of AI response: {ex.Message}");
        }
    }

    #endregion
}
