using UnityEngine;

public class InventorySlot : MonoBehaviour
{
   
    public ItemData item;
    
    public bool IsEmtpy => item == null;

    public void Clear()
    {
        item = null;
    }
}
