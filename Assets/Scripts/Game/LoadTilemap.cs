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
    public TileInfo[] tileInfo;

    Tile[] tiles;

    public string loadFile;

    void Start()
    {
        maps = new Dictionary<TileGroup, Tilemap>
        {
            {TileGroup.ForegroundBasic, GameObject.Find("Grid/Tilemap_Foreground").GetComponent<Tilemap>()}
        };

        SetupTiles();

        WorldGen(100);
    }

    void SetupTiles()
    {
        tiles = new Tile[tileInfo.Length];
        for (int i = 0; i < tileInfo.Length; ++i)
        {
            tiles[i] = ScriptableObject.CreateInstance<Tile>();
            tiles[i].name = tileInfo[i].name;
            tiles[i].sprite = tileInfo[i].sprite;
        }
    }

    readonly float factor1 = 30;
    readonly float factor2 = 11;
    void WorldGen(long seed)
    {
        Vector2Int vecSeed = new((int)(seed), (int)(seed >> 32));
        Vector2Int worldSize = new(100, 30);

        for (int i = -worldSize.x; i <= worldSize.x; ++i)
        {
            print("i = " + i);
            for (int j = 0; j < 10; ++j)
            {
                float x = i + j / 10f;
                Debug.DrawRay(new Vector3(x, Mathf.PerlinNoise(vecSeed.x + x / factor1, vecSeed.y) * factor2  - factor2 / 2f), Vector3.up * 0.1f, Color.red);
            }
            float height2 = Mathf.PerlinNoise(vecSeed.x + i / factor1, vecSeed.y) * factor2 - factor2 / 2f;
            print("Height: " + height2);
            int height = Mathf.RoundToInt(height2);
            maps[TileGroup.ForegroundBasic].SetTile(new Vector3Int(i, height), tiles[0]);
            for (int j = height - 1; j >= -worldSize.y; --j)
            {
                maps[TileGroup.ForegroundBasic].SetTile(new Vector3Int(i, j), tiles[1]);
            }
        }
    }
    void Update()
    {
        //maps[TileGroup.ForegroundBasic].ClearAllTiles();
        //WorldGen();
    }
}
