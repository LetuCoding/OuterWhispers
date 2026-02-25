using System.Collections.Generic;
using UnityEngine;
using InventoryScripts;
using Items;
using SaveSystem;


    public class OuterWhispersSaveSystem
    {
        SimpleSaveSystem saveSystem = new SimpleSaveSystem();

        public void saveData(Player player, Inventory playerInventory)
        {
            PlayerPositionData data = new PlayerPositionData
            {
                position = new float[]
                    { player.transform.position.x, player.transform.position.y, player.transform.position.z },
                playerCurrentHealth = player._healthComponent.CurrentHealth,
                inventory = new List<InventorySlotSaveData>()



            };

            foreach (var slot in playerInventory.items)
            {

                InventorySlot inventorySlot = new InventorySlot
                {
                    item = slot.item,
                };

                data.inventory.Add(new InventorySlotSaveData()
                {
                    itemGuid = slot.IsEmpty ? null : slot.item.Guid
                });
            }
            
            saveSystem.Save(data);
            
        }

        public void LoadData(Player player, Inventory playerInventory, ItemDatabase itemDatabase)
        {
            
            
        
        PlayerPositionData data =  saveSystem.Load<PlayerPositionData>();;

        // Restaurar posición
        player.transform.position = new Vector3(data.position[0], data.position[1], data.position[2]);

        // Restaurar vida
        player._healthComponent.SetHealth(data.playerCurrentHealth);

        // Limpiar inventario
        playerInventory.items.Clear();

        // Asegurarte de que el inventario tenga suficientes slots
        while (playerInventory.items.Count < data.inventory.Count)
        {
            playerInventory.items.Add(new InventorySlot());
        }

        for (int i = 0; i < data.inventory.Count; i++)
        {
            string guid = data.inventory[i].itemGuid;
            ItemData item = itemDatabase.GetItemByGUID(guid);

            playerInventory.items[i].item = item;
        }
        
        }
        
        
        
        
        
        
    }

        
        