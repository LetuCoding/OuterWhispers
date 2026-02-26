using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

namespace InventoryScripts
{
    public class Inventory : MonoBehaviour
    {
        [Header("Inventory settings")] [SerializeField]
        private int slotCount;

        [SerializeField] private Player owner;
        public Player Owner => owner;

        public List<InventorySlot> items = new List<InventorySlot>();

        public event Action<Inventory> EAddItem;


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
                    EAddItem?.Invoke(this);
                    return true;
                    
                }

               
            }

            
            return false;
        }


        public void RemoveItem(InventorySlot slot)
        {
            slot.Clear();

        }
        
        
        

        public bool AddItemSilent(ItemData item)
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

        public bool CheckItem(ItemData item)
        {
            foreach (var slot in items)
            {
                if (slot.item == item) return true;
            }
            
            return false;
        }

        public bool CheckItemByName(String name)
        {
            foreach (var slot in items)
            {
                if (slot.item.name == name) return true;
            }
            return false;
        }

        public bool RemoveItemByName(String name)
        {
            foreach (var slot in items)
            {
                if (slot.item.name == name)
                {
                    items.Remove(slot);
                    Debug.Log("Successfully removed item " + name);
                }
                return true;
            }
            Debug.Log("No such item");
            return false;
        }


    }
}