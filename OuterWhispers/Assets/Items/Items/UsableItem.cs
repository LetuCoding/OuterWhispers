using UnityEngine;

[CreateAssetMenu(menuName = "Items/Usable Item")]
public class UsableItemData : ItemData
{
    [Header("Effect")]
    [SerializeField] private int value;
    [SerializeField] private bool consumeOnUse = true;

    public int Value => value;
    public bool ConsumeOnUse => consumeOnUse;
}