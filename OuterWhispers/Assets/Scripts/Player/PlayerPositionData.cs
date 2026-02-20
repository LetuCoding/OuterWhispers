using UnityEngine;
using System;
using System.Collections.Generic;
using InventoryScripts;



namespace SaveSystem
{
    [Serializable]
    public class PlayerPositionData
    {
        
        public List<InventorySlotSaveData> inventory;
        public float[] position; // x, y, z
        public float playerCurrentHealth;
        public float maxHealth;
        
    }
}
