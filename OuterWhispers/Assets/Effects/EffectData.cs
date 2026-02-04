using Interfaces;
using UnityEngine;


public abstract class EffectData : ScriptableObject
{
    public abstract void Apply(IEffectTarget target);
}
