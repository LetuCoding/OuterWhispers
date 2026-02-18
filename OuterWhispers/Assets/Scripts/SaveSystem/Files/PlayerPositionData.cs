using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEditor.Build.Content;
using UnityEditor.VersionControl;


namespace SaveSystem
{
    [Serializable]
    public class PlayerPositionData
    {
        
        public List<String> itemsId;
        public float[] position; // x, y, z
        public float PlayerCurrentHealth;
        public float maxHealth;
        
    }
}
