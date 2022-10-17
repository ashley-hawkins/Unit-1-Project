using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
public enum TileGroup
{
    Background,
    Midground,
    ForegroundBasic,
    ForegroundPlatform
}

[System.Serializable]
public class TileInfo
{
    public Sprite sprite;
    public string name;
}

public class LoadTilemap : MonoBehaviour
{
    // Start is called before the first frame update
    Dictionary<TileGroup, Tilemap> maps;
    public TileInfo[] tiles;
    void Start()
    {
        maps = new Dictionary<TileGroup, Tilemap>
        {
            {TileGroup.ForegroundBasic, GameObject.Find("Grid/Tilemap_Foreground").GetComponent<Tilemap>()}
        };

        Tile tile = ScriptableObject.CreateInstance<Tile>();
        tile.name = tiles[0].name;
        tile.sprite = tiles[0].sprite;

        for (int i = 0; i < 20; i++)
            maps[TileGroup.ForegroundBasic].SetTile(new Vector3Int(i - 10, -5, 0), tile);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
