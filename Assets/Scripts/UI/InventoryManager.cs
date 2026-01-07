using System;
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

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void AddGold(int amount)
    {
        if (amount <= 0) return;

        gold += amount;
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
}
