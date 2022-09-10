using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapStack : MonoBehaviour
{
    [SerializeField] private List<Tilemap> tileMaps;

    public Tilemap GetTileMap(string stageName)
    {
        if (stageName.Length <= 4)
        {
            return null;
        }
        string numStr = stageName.Substring(5);
        int num = -1;
        if (int.TryParse(numStr, out num))
        {
            return tileMaps[num - 1];
        }
        else
        {
            return null;
        }
    }
}
