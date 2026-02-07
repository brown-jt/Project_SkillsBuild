[System.Serializable]
public class InventorySlot
{
    public ItemData itemData;
    public int quantity;

    public bool IsEmpty => itemData == null;
}
