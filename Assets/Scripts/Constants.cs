using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Constants : MonoBehaviour
{
    public Color[] tileColor;

    public Color GetColor(Tile.EType type)
    {
        return tileColor[(int)type];
    }

    public static string CELL_PREFAB = "Prefabs/Cell"; 
    public static string TILE_PREFAB = "Prefabs/Tile";
}
