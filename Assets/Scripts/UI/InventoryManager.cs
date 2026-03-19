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

    // Currencies
    // TODO: Store in database in future
    private int gold;
    public int Gold => gold;

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
    }

    public void AddGold(int amount)
    {
        if (amount <= 0) return;

        gold += amount;
        goldText.text = gold.ToString();
        OnInventoryChanged?.Invoke();
    }

    public bool RemoveGold(int amount)
    {
        if (amount <= 0) return false;
        if (gold < amount) return false;

        gold -= amount;
        OnInventoryChanged?.Invoke();
        return true;
    }

    public bool AddItem(ItemData item, int amount = 1)
    {
        bool added = DatabaseManager.Instance.AddToInventory(item, amount);

        if (added)
        {
            RefreshInventorySlotsFromDB();
            OnInventoryChanged?.Invoke();
        }

        return added;
    }
    
    public bool HasItem(ItemData item, int amount = 1)
    {
        var inventoryDict = DatabaseManager.Instance.LoadInventory();

        if (inventoryDict.TryGetValue(item, out var stacks))
        {
            int total = 0;
            foreach (var stack in stacks)
            {
                total += stack.amount;
            }
            return total >= amount;
        }

        return false;
    }

    public bool RemoveItem(ItemData item, int amount = 1)
    {
        bool removed = DatabaseManager.Instance.RemoveFromInventory(item, amount);

        if (removed)
        {
            RefreshInventorySlotsFromDB();
            OnInventoryChanged?.Invoke();
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
            ItemData item = kvp.Key;
            var stacks = kvp.Value;

            foreach (var stack in stacks)
            {
                // Map slot_index to UI slot (capped to maxInventorySize)
                int uiSlot = stack.slot_index-1 % maxInventorySize;

                inventorySlots[uiSlot].itemData = item;
                inventorySlots[uiSlot].quantity = stack.amount;
            }
        }

        InventoryUI.Instance.RefreshAll();
    }
}
