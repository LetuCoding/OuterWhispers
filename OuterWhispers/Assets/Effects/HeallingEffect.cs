using Interfaces;
using UnityEngine;

[CreateAssetMenu(fileName = "HeallingEffect", menuName = "EffectData/HeallingEffect")]
public class HeallingEffect : EffectData
{
    [SerializeField] private int healAmount;

    public override void Apply(IEffectTarget target)
    {
        if (target is HealthComponent)
        {
           target.Heal(healAmount);
        }
    }
}
