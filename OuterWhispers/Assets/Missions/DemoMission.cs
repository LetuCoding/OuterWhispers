using InventoryScripts;
using UnityEngine;

public class DemoMission : MonoBehaviour
{
    [SerializeField] private Inventory inventory;

    [SerializeField] private ItemData keyPart1; 
    [SerializeField] private ItemData keyPart2;
    [SerializeField] private ItemData keyPart3;
    [SerializeField] private ItemData combinedItem;

    private bool combined;

    private void OnEnable()
    {
        inventory.EAddItem += CheckCombine;
    }

    private void OnDisable()
    {
        inventory.EAddItem -= CheckCombine;
    }

    void CheckCombine(Inventory inventory)
    {
        if (combined) return;

        if (inventory.CheckItem(keyPart1) &&
            inventory.CheckItem(keyPart2) &&
            inventory.CheckItem(keyPart3))
        {
            combined = true; // 🔑 antes de mutar

            RemoveItem(inventory, keyPart1);
            RemoveItem(inventory, keyPart2);
            RemoveItem(inventory, keyPart3);

            inventory.AddItemSilent(combinedItem);

            Debug.Log("✨ Misión completada: ítems combinados");
        }
    }

    void RemoveItem(Inventory inventory, ItemData item)
    {
        foreach (var slot in inventory.items)
        {
            if (slot.item == item)
            {
                slot.Clear();
                return;
            }
        }
    }
}