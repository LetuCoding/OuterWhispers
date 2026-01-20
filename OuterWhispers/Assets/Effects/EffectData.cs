using Interfaces;
using UnityEngine;

[CreateAssetMenu(menuName = "EffectData")]
public abstract class EffectData : ScriptableObject
{
    public abstract void Apply(IEffectTarget target);
}
