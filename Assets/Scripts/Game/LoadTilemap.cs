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
        Vector2Int worldSize = new(50, 50);
        // 2000x1000 total, 1000 in both directions and 500 in both directions
        worldSize *= 2;

        bool[][,] caveMap = new bool[2][,] {
            new bool [worldSize.x, worldSize.y],
            new bool [worldSize.x, worldSize.y],
        };

        for (int i = 0; i < worldSize.x; ++i)
        {
            for (int j = 0; j < worldSize.y; ++j)
            {
                bool b = (Mathf.PerlinNoise(i / factor1, j / factor1)) > 0.5f;
                caveMap[0][i, j] = b;
                if (b)
                {
                    //maps[TileGroup.ForegroundBasic].SetTile(new Vector3Int(0,0), tiles[0]);
                    maps[TileGroup.ForegroundBasic].SetTile(new Vector3Int(i - worldSize.x / 2, j - worldSize.y / 2), tiles[0]);
                }
            }
        }


        // for (int i = -worldSize.x; i <= worldSize.x; ++i)
        // {
        //     print("i = " + i);
        //     for (int j = 0; j < 10; ++j)
        //     {
        //         float x = i + j / 10f;
        //         Debug.DrawRay(new Vector3(x, Mathf.PerlinNoise(vecSeed.x + x / factor1, vecSeed.y) * factor2  - factor2 / 2f), Vector3.up * 0.1f, Color.red);
        //     }
        //     float height2 = Mathf.PerlinNoise(vecSeed.x + i / factor1, vecSeed.y) * factor2 - factor2 / 2f;
        //     print("Height: " + height2);
        //     int height = Mathf.RoundToInt(height2);
        //     maps[TileGroup.ForegroundBasic].SetTile(new Vector3Int(i, height), tiles[0]);
        //     for (int j = height - 1; j >= -worldSize.y; --j)
        //     {
        //         maps[TileGroup.ForegroundBasic].SetTile(new Vector3Int(i, j), tiles[1]);
        //     }
        // }
    }
    void Update()
    {
        //maps[TileGroup.ForegroundBasic].ClearAllTiles();
        //WorldGen();
    }








    // Taken from https://blog.unity.com/technology/procedural-patterns-to-use-with-tilemaps-part-ii







    static int GetMooreSurroundingTiles(int[,] map, int x, int y, bool edgesAreWalls)
    {
        /* Moore Neighbourhood looks like this ('T' is our tile, 'N' is our neighbours)
        *
        * N N N
        * N T N
        * N N N
        *
        */

        int tileCount = 0;

        for(int neighbourX = x - 1; neighbourX <= x + 1; neighbourX++)
        {
            for(int neighbourY = y - 1; neighbourY <= y + 1; neighbourY++)
            {
                if (neighbourX >= 0 && neighbourX < map.GetUpperBound(0) && neighbourY >= 0 && neighbourY < map.GetUpperBound(1))
                {
                    //We don't want to count the tile we are checking the surroundings of
                    if(neighbourX != x || neighbourY != y)
                    {
                        tileCount += map[neighbourX, neighbourY];
                    }
                }
            }
        }
        return tileCount;
    }

    public static int[,] SmoothMooreCellularAutomata(int[,] map, bool edgesAreWalls, int smoothCount)
    {
        for (int i = 0; i < smoothCount; i++)
        {
            for (int x = 0; x < map.GetUpperBound(0); x++)
            {
                for (int y = 0; y < map.GetUpperBound(1); y++)
                {
                    int surroundingTiles = GetMooreSurroundingTiles(map, x, y, edgesAreWalls);

                    if (edgesAreWalls && (x == 0 || x == (map.GetUpperBound(0) - 1) || y == 0 || y == (map.GetUpperBound(1) - 1)))
                    {
                        //Set the edge to be a wall if we have edgesAreWalls to be true
                        map[x, y] = 1;
                    }
                    //The default moore rule requires more than 4 neighbours
                    else if (surroundingTiles > 4)
                    {
                        map[x, y] = 1;
                    }
                    else if (surroundingTiles < 4)
                    {
                        map[x, y] = 0;
                    }
                }
            }
        }
        //Return the modified map
        return map;
    }



}
