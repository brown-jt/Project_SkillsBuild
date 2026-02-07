using UnityEngine;

[CreateAssetMenu(menuName = "Inventory/Item")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    public string description;

    public bool stackable;
    public int maxStack = 99; // Default max stack size if stackable

    // Optional prefab for world representation
    public GameObject prefab; 
}
