using Interfaces;
using UnityEngine;

[CreateAssetMenu(fileName = "EffectData", menuName = "Scriptable Objects/EffectData")]
public abstract class EffectData : ScriptableObject
{
    public abstract void Apply(IEffectTarget target);
}
