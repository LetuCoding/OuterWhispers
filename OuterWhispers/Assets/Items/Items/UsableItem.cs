using Interfaces;
using UnityEngine;

[CreateAssetMenu(fileName = "UsableItem", menuName = "Items/UsableItem")]
public class UsableItemData : ItemData
{
    [Header("Effect")]
    
    public EffectData effect;
    
    [SerializeField] private bool consumeOnUse = true;
    
    public bool ConsumeOnUse => consumeOnUse;

    

    public void Use(IEffectTarget target)
    {
        effect.Apply(target);    
    }
    
}