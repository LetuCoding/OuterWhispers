using UnityEngine;

public abstract class UsableItemData : ItemData
{
    [Header("Effect")]
    
    [SerializeField] private bool consumeOnUse = true;
    
    public bool ConsumeOnUse => consumeOnUse;


    public abstract void Use();
}