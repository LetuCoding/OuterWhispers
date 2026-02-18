using System.Collections.Generic;
using UnityEngine;
using System.IO;
using InventoryScripts;
using SaveSystem;

public class SimpleSaveSystem
{
    private string GetPath() => Path.Combine(Application.persistentDataPath, "player_position.json");

    public void Save(Player player, Inventory playerInventory)
    {
        PlayerPositionData data = new PlayerPositionData
        {

            position = new float[] { player.transform.position.x, player.transform.position.y, player.transform.position.z },
            PlayerCurrentHealth = player._healthComponent.CurrentHealth,
            itemsId = new List<InventorySlot>()
            
            
            
        };
        foreach (var slot in playerInventory.items)
        {
            InventorySlot inventorySlot = new InventorySlot
            {
                item = slot.item,
            };
            
            data.itemsId.Add(inventorySlot);
        }
        
        string json = JsonUtility.ToJson(data, true); // pretty print
        File.WriteAllText(GetPath(), json);

        Debug.Log($"[SAVE] Player position saved at {GetPath()}");
    }

    public Vector3 Load()
    {
        string path = GetPath();
        if (!File.Exists(path))
        {
            Debug.LogWarning("[LOAD] Save file not found");
            return Vector3.zero; // fallback
        }

        string json = File.ReadAllText(path);
        PlayerPositionData data = JsonUtility.FromJson<PlayerPositionData>(json);

        return new Vector3(data.position[0], data.position[1], data.position[2]);
    }
}