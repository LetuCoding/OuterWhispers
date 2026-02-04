using Interfaces;
using UnityEngine;

[CreateAssetMenu(fileName = "SpeedBuffEffect", menuName = "EffectData/SpeedBuffEffect")]
public class SpeedBuffEffect :  EffectData
{
    public override void Apply(IEffectTarget target)
    {
        if (target is Player player)
        {
            Debug.Log(player.speed);
            player.speed *= 2f;
            
            Debug.Log(player.speed+ " after");
        }
    }
}
