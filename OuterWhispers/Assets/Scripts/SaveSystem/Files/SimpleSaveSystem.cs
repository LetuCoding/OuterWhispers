using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SimpleSaveSystem
{
    private string GetPath() => Path.Combine(Application.persistentDataPath, "player_position.json");

    public void Save<T>(T data)
    {
        
        string json = JsonUtility.ToJson(data, true); // pretty print
        File.WriteAllText(GetPath(), json);
        JsonUtility.FromJson<T>(json);
        Debug.Log($"[SAVE] Player position saved at {GetPath()}");
    }

    public T Load<T>()
    {
        //Player player, Inventory playerInventory, ItemDatabase itemDatabase
        string path = GetPath();
        
        if (!File.Exists(path))
        {
            Debug.LogWarning("[LOAD] Save file not found.");
         
        }
        

        T data = JsonUtility.FromJson<T>(File.ReadAllText(path));
        
        Debug.Log("[LOAD] Player loaded successfully!");
        return data ; 
    }
}