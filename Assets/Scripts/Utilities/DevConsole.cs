using System;
using System.Collections.Generic;
using UnityEngine;

public class DevConsole : MonoBehaviour
{
    public static DevConsole Instance;
    public DevConsoleUI ui;

    // Runtime items dictionary for quick lookup
    private Dictionary<string, ItemData> _itemsDictionary;

    Dictionary<string, Action<string[]>> commands = new Dictionary<string, Action<string[]>>();
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        RegisterCommands();
        InitItemsDictionary();
    }

    private void RegisterCommands()
    {
        commands.Add("heal", Heal);
        commands.Add("give_gold", GiveGold);
        commands.Add("give_item", GiveItem);

        // Add a command to list all available items
        commands.Add("list_items", args =>
        {
            Log("Available items:");
            foreach (var item in _itemsDictionary.Values)
            {
                Log($"{item.itemId}");
            }
        });
    }

    private void InitItemsDictionary()
    {
        _itemsDictionary = new Dictionary<string, ItemData>();

        // Load item scriptable objects from Resources folder
        ItemData[] allItems = Resources.LoadAll<ItemData>("Items");

        foreach (var item in allItems)
        {
            if (!_itemsDictionary.ContainsKey(item.itemId)) _itemsDictionary[item.itemId] = item;
            else Debug.LogWarning($"Duplicate item ID found: {item.itemId}. Skipping.");
        }
    }

    public void Execute(string input)
    {
        var args = input.Split(' ');
        var command = args[0];
        var parameters = args.Length > 1 ? args[1..] : Array.Empty<string>();

        if (commands.TryGetValue(command, out var action))
        {
            action(parameters);
        }
        else
        {
            Log($"Unknown command: {command}");
        }
    }

    private void Log(string msg)
    {
        Debug.Log($"[CONSOLE] {msg}");
        ui.Log($"[CONSOLE] {msg}");
    }

    private void Heal(string[] args)
    {
        int quantity = int.Parse(args[0]);
        // TODO: Actually heal the player

        Log($"Healed player for {quantity}");
    }

    private void GiveGold(string[] args)
    {
        int quantity = int.Parse(args[0]);
        InventoryManager.Instance.AddGold(quantity);

        Log($"Added {quantity} gold");
    }

    private void GiveItem(string[] args)
    {
        // Check argument count
        if (args.Length < 2)
        {
            Log("Error: Missing arguments. Usage: GiveItem <itemName> <quantity>");
            return;
        }

        string itemName = args[0];

        // Validate quantity
        if (!int.TryParse(args[1], out int quantity))
        {
            Log($"Error: Invalid quantity. '{args[1]}' must be a number.");
            return;
        }

        if (quantity <= 0)
        {
            Log($"Error: Quantity must be greater than 0. Given: {quantity}");
            return;
        }

        // Try to get item
        if (!_itemsDictionary.TryGetValue(itemName, out var itemToAdd) || itemToAdd == null)
        {
            Log($"Error: Item not found: {itemName}");
            return;
        }

        // Add item
        InventoryManager.Instance.AddItem(itemToAdd, quantity);
        Log($"Added {quantity}x {itemToAdd.itemName}");
        FeedbackNotificationsUI.Instance.AddNotification($"Added {quantity}x {itemToAdd.itemName}");
    }
}
