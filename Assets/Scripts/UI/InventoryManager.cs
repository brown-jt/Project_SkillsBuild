using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    // Singleton
    public static InventoryManager Instance;

    // Events
    public event Action OnInventoryChanged;

    // References
    [Header("References")]
    [SerializeField] private TextMeshProUGUI goldText;

    // Inventory UI slots for rendering and drag/drop functionality
    public List<InventorySlot> inventorySlots = new List<InventorySlot>();
    public int maxInventorySize = 25;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        // Initialize inventory with empty slots
        for (int i = 0; i < maxInventorySize; i++)
        {
            inventorySlots.Add(new InventorySlot());
        }
    }

    private void Start()
    {
        RefreshInventorySlotsFromDB();
        InventoryUI.Instance.RefreshAll();
        goldText.text = DatabaseManager.Instance.GetGold().ToString();
    }

    public void AddGold(int quantity)
    {
        if (quantity <= 0) return;

        if (DatabaseManager.Instance.AddGold(quantity))
        {
            int gold = DatabaseManager.Instance.GetGold();
            goldText.text = gold.ToString();
            OnInventoryChanged?.Invoke();
        }
    }

    public bool RemoveGold(int quantity)
    {
        if (quantity <= 0) return false;

        int gold = DatabaseManager.Instance.GetGold();
        if (gold < quantity) return false;

        if (DatabaseManager.Instance.RemoveGold(quantity))
        {
            goldText.text = gold.ToString();
            OnInventoryChanged?.Invoke();
            return true;
        }
        return false;
    }

    public bool AddItem(ItemData item, int quantity = 1)
    {
        int slotIndexToAdd = GetFirstEmptySlotIndex();

        if (slotIndexToAdd == -1)
        {
            Debug.LogWarning("No empty inventory slot available!");
            return false;
        }

        bool added = DatabaseManager.Instance.AddToInventory(item, slotIndexToAdd, quantity);

        if (added)
        {
            RefreshInventorySlotsFromDB();
            OnInventoryChanged?.Invoke();
        }

        return added;
    }
    
    public bool HasItem(ItemData item, int quantity = 1)
    {
        var inventoryDict = DatabaseManager.Instance.LoadInventory();

        if (inventoryDict.TryGetValue(item.itemId, out var stacks))
        {
            int total = 0;
            foreach (var stack in stacks)
            {
                total += stack.quantity;
            }
            return total >= quantity;
        }

        return false;
    }

    public bool RemoveItem(ItemData item, int quantity = 1)
    {
        bool removed = DatabaseManager.Instance.RemoveFromInventory(item, quantity);

        if (removed)
        {
            RefreshInventorySlotsFromDB();
            OnInventoryChanged?.Invoke();
            FeedbackNotificationsUI.Instance.AddNotification($"Removed {quantity}x {item.itemName}");
        }

        return removed;
    }

    public void RefreshInventorySlotsFromDB()
    {
        // Firstly clear all slots
        foreach (var slot in inventorySlots)
        {
            slot.Clear();
        }

        // Load each inventory item from database and populate slots
        var inventoryDict = DatabaseManager.Instance.LoadInventory();

        foreach (var kvp in inventoryDict)
        {
            string itemId = kvp.Key;
            var stacks = kvp.Value;

            // Get item data from database runtime dictionary
            if (!DatabaseManager.Instance.ItemsDict.TryGetValue(itemId, out var item))
            {
                Debug.LogError($"Item ID {itemId} not found in item data dictionary!");
                continue;
            }

            foreach (var stack in stacks)
            {
                // Map slot_index to UI slot (capped to maxInventorySize)
                int uiSlot = stack.slot_index % maxInventorySize;

                inventorySlots[uiSlot].itemData = item;
                inventorySlots[uiSlot].quantity = stack.quantity;
            }
        }

        InventoryUI.Instance.RefreshAll();
    }

    private int GetFirstEmptySlotIndex()
    {
        for (int i = 0; i < inventorySlots.Count; i++)
        {
            if (inventorySlots[i].IsEmpty)
                return i;
        }

        return -1; // No empty slot
    }
}
