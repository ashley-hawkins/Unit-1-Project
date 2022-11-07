using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Unity.Mathematics;

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

    void doCircle(bool[,] caveMap, Vector2Int pos, int radius)
    {
        float minAngle = Mathf.Acos(1f - 1f / radius);
        for (float angle = 0; angle < 360; angle += minAngle)
        {
            int circumX = pos.x + Mathf.RoundToInt(Mathf.Cos(angle) * radius);
            int circumY = pos.y + Mathf.RoundToInt(Mathf.Sin(angle) * radius);

            for (int x = pos.x; Mathf.Sign(x - circumX) == Mathf.Sign(pos.x - circumX); x -= Mathf.RoundToInt(Mathf.Sign(pos.x - circumX)))
            {
                if (0 <= x && x <= caveMap.GetUpperBound(0) && 0 <= circumY && circumY <= caveMap.GetUpperBound(1))
                    caveMap[x, circumY] = false;
            }
        }
    }

    void doCave(bool[,] caveMap, Vector2Int pos, int length, int radius)
    {
        for (int i = 0; i < length; ++i)
        {
            doCircle(caveMap, pos + new Vector2Int(Mathf.RoundToInt((Mathf.PerlinNoise((pos.y + i) / 5f / radius, pos.x / 5f / radius) - 0.5f) * 30f * radius), i), radius);
        }
    }

    void doCaveRandom(bool[,] caveMap, Vector2Int pos)
    {
        int length = UnityEngine.Random.Range(200, 300);
        int radius = UnityEngine.Random.Range(8, 15);
    }

    void WorldGen(long seed)
    {
        var rngOldState = UnityEngine.Random.state;
        UnityEngine.Random.InitState((int)seed);
        Vector2Int vecSeed = new((int)(seed), (int)(seed >> 32));
        Vector2Int worldSize = new(500, 300);
        // 2000x1000 total, 1000 in both directions and 500 in both directions
        worldSize *= 2;

        bool[,] caveMap = new bool[worldSize.x, worldSize.y];
        int[] heightMap = new int[worldSize.x];

        for (int i = 0; i <= worldSize.x; ++i)
        {
            for (int j = 0; j < 10; ++j)
            {
                float x = i + j / 10f;
                Debug.DrawRay(new Vector3(x, (Mathf.PerlinNoise(vecSeed.x + x / factor1, vecSeed.y) - 0.5f) * factor2 - factor2 / 2f), Vector3.up * 0.1f, Color.red);
            }
            float noiseHeight = (Mathf.PerlinNoise(vecSeed.x + i / factor1, vecSeed.y) - 0.5f) * factor2;
            int height = Mathf.RoundToInt(noiseHeight);
            maps[TileGroup.ForegroundBasic].SetTile(new Vector3Int(i, height), tiles[0]);
            for (int j = height - 1; j >= -worldSize.y; --j)
            {
                maps[TileGroup.ForegroundBasic].SetTile(new Vector3Int(i, j), tiles[1]);
            }
        }

        for (int i = 0; i < worldSize.x; ++i)
        {
            for (int j = 0; j < worldSize.y; ++j)
            {
                // bool b = (Mathf.PerlinNoise(i / 30.0f, j / 30.0f) + Mathf.PerlinNoise(i / 20.0f, j / 20.0f) * 0.6f) / 1.6f > 0.35f;
                // bool b = UnityEngine.Random.Range(0f, 1f) > 0.45f;
                caveMap[i, j] = true;
            }
        }

        for (int i = 0; i < worldSize.x; ++i)
        {
           for (int j = 0; j < worldSize.y; ++j)
           {
               bool shouldntCave = UnityEngine.Random.Range(0f, 1f) > 0.00001f;
               if (!shouldntCave)
                   doCaveRandom(caveMap, new Vector2Int(i, j)); // some sort of brownian motion type thing that draws circles and stuff
           }
        }

        doCave(caveMap, new Vector2Int(500, 600), 350, 15);

        //caveMap = SmoothMooreCellularAutomata(caveMap, true, 4);

        for (int i = 0; i < worldSize.x; ++i)
        {
            for (int j = 0; j < worldSize.y; ++j)
            {
                if (caveMap[i, j])
                    maps[TileGroup.ForegroundBasic].SetTile(new Vector3Int(i, j), tiles[0]);
            }
        }

        UnityEngine.Random.state = rngOldState;
    }
    void Update()
    {
        //maps[TileGroup.ForegroundBasic].ClearAllTiles();
        //WorldGen();
    }








    // Adapted from https://blog.unity.com/technology/procedural-patterns-to-use-with-tilemaps-part-ii

    static int GetMooreSurroundingTiles(bool[,] map, int x, int y, bool edgesAreWalls)
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
                        tileCount += map[neighbourX, neighbourY] ? 1 : 0;
                    }
                }
            }
        }
        return tileCount;
    }

    public static bool[,] SmoothMooreCellularAutomata(bool[,] mapA, bool edgesAreWalls, int smoothCount)
    {
        bool[,] mapB = new bool[mapA.GetLength(0), mapA.GetLength(1)];
        for (int i = 0; i < smoothCount; i++)
        {
            for (int x = 0; x <= mapA.GetUpperBound(0); x++)
            {
                for (int y = 0; y <= mapA.GetUpperBound(1); y++)
                {
                    int surroundingTiles = GetMooreSurroundingTiles(mapA, x, y, edgesAreWalls);

                    if (edgesAreWalls && (x == 0 || x == mapA.GetUpperBound(0) || y == 0 || y == mapA.GetUpperBound(1)))
                    {
                        //Set the edge to be a wall if we have edgesAreWalls to be true
                        mapB[x, y] = true;
                    }
                    //The default moore rule requires more than 4 neighbours
                    else if (surroundingTiles > 4)
                    {
                        mapB[x, y] = true;
                    }
                    else if (surroundingTiles < 4)
                    {
                        mapB[x, y] = false;
                    }
                    else
                    {
                        mapB[x, y] = mapA[x, y];
                    }
                }
            }
            (mapA, mapB) = (mapB, mapA);
        }
        //Return the modified map
        return mapA;
    }



}
