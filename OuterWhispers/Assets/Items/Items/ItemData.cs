using UnityEngine;

public enum ItemType
{
    Usable,
    KeyItem,
    Quest
}


public abstract class ItemData : ScriptableObject
{
    [Header("Info")]
    [SerializeField] private string guid = System.Guid.NewGuid().ToString();
    
    [SerializeField] private string itemName;
    [TextArea(2, 3)]
    [SerializeField] private string description;
    [SerializeField] private Sprite icon;
    [SerializeField] private ItemType itemType;

    public string ItemName => itemName;
    public string Description => description;
    public Sprite Icon => icon;
    public ItemType Type => itemType;
    
    public string Guid => guid;
}