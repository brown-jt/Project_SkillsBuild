[System.Serializable]
public class InventorySlot
{
    public ItemData itemData;
    public int quantity;

    public bool IsEmpty => itemData == null;

    public void Clear()
    {
        itemData = null;
        quantity = 0;
    }
}
