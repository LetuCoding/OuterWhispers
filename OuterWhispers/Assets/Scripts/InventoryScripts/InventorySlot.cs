[System.Serializable]
public class InventorySlot
{
    public ItemData item;

    public bool IsEmpty => item == null;

    public void Clear()
    {
        item = null;
    }
}