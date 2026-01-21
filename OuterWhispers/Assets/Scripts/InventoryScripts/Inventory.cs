using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace InventoryScripts
{
    public class Inventory : MonoBehaviour
    {
        [Header("Inventory settings")]
        
        [SerializeField]private int slotCount;
        
        [SerializeField]private GameObject owner;
        public GameObject Owner => owner;
        
        public List<InventorySlot>  items = new List<InventorySlot>();

        
        private void Awake()
        {
            items.Clear();

            for (int i = 0; i < slotCount; i++)
            {
                items.Add(new InventorySlot());
            }
        }

        public bool AddItem(ItemData item)
        {
            foreach (var slot in items)
            {
                if (slot.IsEmpty)
                {
                    slot.item = item;
                    return true;
                }
                
            }
            return false;
        }


        public void RemoveItem(InventorySlot slot)
        {
            slot.Clear();
            
        }


        
    }
}