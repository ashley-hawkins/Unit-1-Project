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

    float GetSimplexNoise(float x, float y, int octaves = 1)
    {
        float2 pos = new(x, y);
        float res = 0f;
        float max = 0f;
        float mult = 1f;
        for (int i = 0; i < octaves; i++)
        {
            res += noise.snoise(pos) * mult;
            max += mult;
            mult *= 0.5f;
            pos /= 0.5f;
        }
        return res / max;
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
    readonly float factor2 = 7;

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
        Vector2 lastOffset = Vector2.right * GetSimplexNoise(pos.y / 5f / radius, pos.x / 5f / radius) * 5f * radius;
        for (int i = 0; i < length; ++i)
        {
            float theXCoordinate = ((GetSimplexNoise((pos.y - i) / 5f / radius, pos.x / 5f / radius)) * 5f * radius);

            Vector2 nextOffset = new(theXCoordinate, - i);
            var posDiff = (nextOffset - lastOffset).magnitude;
            if (posDiff != 0)
            {
                for (float l = 0; l < 1; l = Mathf.Min(1, l + 1f / posDiff))
                {
                    doCircle(caveMap, pos + Vector2Int.RoundToInt(Vector2.Lerp(lastOffset, nextOffset, l)), radius);
                }
            }
            else
            {
                doCircle(caveMap, pos + new Vector2Int(Mathf.RoundToInt(theXCoordinate), -i), radius);
            }
            lastOffset = nextOffset;
        }
    }

    void doCaveRandom(bool[,] caveMap, Vector2Int pos)
    {
        int length = UnityEngine.Random.Range(200, 300);
        int radius = UnityEngine.Random.Range(4, 10);
        doCave(caveMap, pos, length, radius);
    }

    void WorldGen(long seed)
    {
        var rngOldState = UnityEngine.Random.state;
        UnityEngine.Random.InitState((int)seed);
        Vector2Int vecSeed = new((int)(seed), (int)(seed >> 32));
        Vector2Int worldSize = new(500, 300);
        // 2000x1000 total, 1000 in both directions and 500 in both directions
        worldSize *= 2;

        bool[,] squiggleCaveMap = new bool[worldSize.x, worldSize.y];
        bool[,] openCaveMap = new bool[worldSize.x, worldSize.y];
        int[] heightMap = new int[worldSize.x];

        for (int i = 0; i < worldSize.x; ++i)
        {
            for (int j = 0; j < 10; ++j)
            {
                float x = i + j / 10f;
                Debug.DrawRay(new Vector3(x, GetSimplexNoise(vecSeed.x + (x / factor1), vecSeed.y, 3) * factor2), Vector3.up * 0.1f, Color.red);
            }
            float noiseHeight = GetSimplexNoise(vecSeed.x + (i / factor1), vecSeed.y, 2) * factor2;
            int height = Mathf.RoundToInt(noiseHeight) + worldSize.y - Mathf.CeilToInt(factor2) - 100;
            heightMap[i] = height;
        }

        for (int i = 0; i < worldSize.x; ++i)
        {
            for (int j = 0; j < worldSize.y; ++j)
            {
                // bool b = (Mathf.PerlinNoise(i / 30.0f, j / 30.0f) + Mathf.PerlinNoise(i / 20.0f, j / 20.0f) * 0.6f) / 1.6f > 0.35f;
                bool b = j > 400 || GetSimplexNoise(i / 128f, j / 128f, 5) < 0.2f;
                openCaveMap[i, j] = b;
                squiggleCaveMap[i, j] = true;
            }
        }

        openCaveMap = SmoothMooreCellularAutomata(openCaveMap, true, 1);

        for (int i = 0; i < worldSize.x; ++i)
        {
           for (int j = 0; j < worldSize.y; ++j)
           {
               bool shouldntCave = UnityEngine.Random.Range(0f, 1f) > 0.00001f;
               if (!shouldntCave)
                   doCaveRandom(squiggleCaveMap, new Vector2Int(i, j));
           }
        }

//        doCircle(squiggleCaveMap, worldSize / 2, 50);

        doCave(squiggleCaveMap, new Vector2Int(worldSize.x / 2, heightMap[worldSize.x / 2]), heightMap[worldSize.x / 2], 5);

        for (int i = 0; i < worldSize.x; ++i)
        {
            for (int y = heightMap[i]; y < worldSize.y; ++y)
            {
                squiggleCaveMap[i, y] = false;
            }
        }

        for (int i = 0; i < worldSize.x; ++i)
        {
            bool first = true;
            for (int j = worldSize.y - 1; j >= 0; --j)
            {
                if (squiggleCaveMap[i, j] && openCaveMap[i,j])
                {
                    if (first)
                    {
                        maps[TileGroup.ForegroundBasic].SetTile(new Vector3Int(i, j), tiles[0]);
                        first = false;
                    }
                    else if (j > 400)
                        maps[TileGroup.ForegroundBasic].SetTile(new Vector3Int(i, j), tiles[1]);
                    else
                        maps[TileGroup.ForegroundBasic].SetTile(new Vector3Int(i, j), tiles[2]);
                }
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
