using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "WallType", menuName = "Board/Wall Type")]
public class WallType : ScriptableObject
{
    [Header("Visual")]
    public Tile[] DamageTiles;

    [Header("Atributos")]
    public int MaxHealth = 3;

}