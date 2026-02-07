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
    private int gold;
    public int Gold => gold;

    // References
    [Header("References")]
    [SerializeField] private TextMeshProUGUI goldText;

    // Inventory
    public List<InventorySlot> inventorySlots = new List<InventorySlot>();
    public int maxInventorySize = 49;

    // test
    public ItemData testItem;
    public ItemData testItem2;

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
        AddItem(testItem, 1);
        AddItem(testItem2, 10);
        AddItem(testItem2, 2);

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
        // Stackable items
        if (item.stackable)
        {
            foreach (var slot in inventorySlots)
            {
                if (slot.itemData == item && slot.quantity < item.maxStack)
                {
                    int spaceLeft = item.maxStack - slot.quantity;
                    int toAdd = Mathf.Min(spaceLeft, amount);
                    slot.quantity += toAdd;
                    amount -= toAdd;
                    if (amount <= 0)
                    {
                        OnInventoryChanged?.Invoke();
                        return true;
                    }
                }
            }
        }

        // Non-stackable or remaining stackable items
        foreach (var slot in inventorySlots)
        {
            if (slot.IsEmpty)
            {
                slot.itemData = item;
                slot.quantity = Mathf.Min(amount, item.stackable ? item.maxStack : 1);
                amount -= slot.quantity;
                if (amount <= 0)
                {
                    OnInventoryChanged?.Invoke();
                    return true;
                }
            }
        }

        // No space left in inventory
        return false;
    }
}
