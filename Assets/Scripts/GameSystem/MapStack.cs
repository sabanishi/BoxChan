using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapStack : MonoBehaviour
{
    [SerializeField] private List<Tilemap> tileMaps;

    public Tilemap GetTileMap(string stageName)
    {
        string numStr = stageName.Substring(5);
        int num = int.Parse(numStr);
        return tileMaps[num-1];
    }
}
