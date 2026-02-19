using UnityEngine;

namespace Items
{
    using System.Collections.Generic;
    using UnityEngine;

    public class ItemDatabase : MonoBehaviour
    {
        public List<ItemData> items;  // todos los ScriptableObjects de ítems del juego

        private Dictionary<string, ItemData> itemDictionary;

        private void Awake()
        {
            if (FindObjectsOfType<ItemDatabase>().Length > 1)
            {
                Destroy(gameObject);
                return;
            }

            DontDestroyOnLoad(gameObject);

            BuildDictionary();
        }

        private void BuildDictionary()
        {
            itemDictionary = new Dictionary<string, ItemData>();

            foreach (var item in items)
            {
                if (itemDictionary.ContainsKey(item.Guid))
                    Debug.LogError($"Duplicate GUID detected: {item.Guid}");
                else
                    itemDictionary.Add(item.Guid, item);
            }
        }
        
        public ItemData GetItemByGUID(string guid)
        {
            if (string.IsNullOrEmpty(guid)) return null;
            return itemDictionary.ContainsKey(guid) ? itemDictionary[guid] : null;
        }
    }

}