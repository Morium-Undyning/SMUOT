using System;
using Unity.AI.Navigation;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class MapGenerator : MonoBehaviour
{
    [Header("Настройки карты")]
    public int mapWidth = 50;
    public int mapHeight = 50;
    public float tileSize = 2f;

    [Header("Префабы тайлов")]
    public GameObject grassPrefab;
    public GameObject dirtPrefab;
    public GameObject stonePrefab;

    [Header("Настройки высоты")]
    public float heightScale = 2f;
    public int heightLevels = 20;
    public float noiseScale = 10f;

    [Header("Настройки биомов")]
    public float dirtThreshold = 0.6f;
    public float stoneThreshold = 0.8f;

    [Header("Настройки шума для высоты")]
    public float heightNoiseScale = 5f;
    public float heightSmoothness = 0.5f;

    [Header("Прочее")]
    Camera camera;
    public GameObject mainTower;
    private GameObject levelFolder;

    private TileData[,] grid;

    void Awake()
    {
        levelFolder = new GameObject();
        levelFolder.name = "Level";
        levelFolder.transform.position = Vector3.zero;

        camera = Camera.main;
        GenerateMap();
    }

    void SetUpCamera()
    {
        camera.transform.position = new Vector3(
            mapWidth * tileSize / 2,
            Mathf.Max(10f, heightScale * 2),
            mapHeight * tileSize / 2
        );
    }

    void PlaceMainTower(Vector3 pos)
    {
        Instantiate(mainTower, pos, Quaternion.identity, transform);
    }

    public void GenerateMap()
    {
        ClearMap();
        SetUpCamera();
        grid = new TileData[mapWidth, mapHeight];

        float offsetX = Random.Range(0f, 100f);
        float offsetY = Random.Range(0f, 100f);
        float heightOffsetX = Random.Range(0f, 100f);
        float heightOffsetY = Random.Range(0f, 100f);

        float[,] heightMap = GenerateHeightMap(offsetX, offsetY, heightOffsetX, heightOffsetY);

        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                float height = heightMap[x, y];

                float surfaceNoise = Mathf.PerlinNoise(
                    (x + offsetX) / noiseScale,
                    (y + offsetY) / noiseScale
                );

                TileType type = GetTileType(surfaceNoise, height);
                CreateTile(x, y, type, surfaceNoise, height);
            }
        }

        Vector3 towerPos = FindSuitableTowerPosition(heightMap);
        PlaceMainTower(towerPos);

        RebuildNavMesh();
    }

    float[,] GenerateHeightMap(float offsetX, float offsetY, float heightOffsetX, float heightOffsetY)
    {
        float[,] heightMap = new float[mapWidth, mapHeight];

        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                float noiseValue = 0f;
                float frequency = 1f;
                float amplitude = 1f;
                float maxAmplitude = 0f;

                for (int i = 0; i < 3; i++)
                {
                    noiseValue += Mathf.PerlinNoise(
                        (x + heightOffsetX) * frequency / heightNoiseScale,
                        (y + heightOffsetY) * frequency / heightNoiseScale
                    ) * amplitude;

                    maxAmplitude += amplitude;
                    amplitude *= heightSmoothness;
                    frequency *= 2f;
                }

                noiseValue /= maxAmplitude;

                float quantizedHeight = Mathf.Round(noiseValue * heightLevels) / heightLevels * heightScale;
                heightMap[x, y] = quantizedHeight;
            }
        }

        return heightMap;
    }

    void ClearMap()
    {
        if (levelFolder != null)
        {
            DestroyImmediate(levelFolder);
        }

        levelFolder = new GameObject();
        levelFolder.name = "Level";
        levelFolder.transform.position = Vector3.zero;

        grid = null;
    }

    TileType GetTileType(float noise, float height)
    {
        float heightFactor = height / heightScale;

        if (noise > stoneThreshold || heightFactor > 0.7f)
            return TileType.Stone;
        if (noise > dirtThreshold || heightFactor > 0.4f)
            return TileType.Dirt;
        return TileType.Grass;
    }

    void CreateTile(int x, int y, TileType type, float noise, float height)
    {
        Vector3 pos = new Vector3(x * tileSize, height, y * tileSize);
        GameObject prefab = GetTilePrefab(type);

        if (prefab)
        {
            GameObject tile = Instantiate(prefab, pos, Quaternion.identity, levelFolder.transform);

            grid[x, y] = new TileData
            {
                gridPosition = new Vector2Int(x, y),
                type = type,
                currentTileObject = tile,
                noiseValue = noise,
                height = height
            };
        }
    }

    GameObject GetTilePrefab(TileType type)
    {
        return type switch
        {
            TileType.Grass => grassPrefab,
            TileType.Dirt => dirtPrefab,
            TileType.Stone => stonePrefab,
            _ => grassPrefab
        };
    }

    Vector3 FindSuitableTowerPosition(float[,] heightMap)
    {
        int centerX = mapWidth / 2;
        int centerZ = mapHeight / 2;

        float avgHeight = 0f;
        int sampleCount = 0;
        int levelingRadius = 2;

        for (int x = centerX - levelingRadius; x <= centerX + levelingRadius; x++)
        {
            for (int z = centerZ - levelingRadius; z <= centerZ + levelingRadius; z++)
            {
                if (x >= 0 && x < mapWidth && z >= 0 && z < mapHeight)
                {
                    avgHeight += heightMap[x, z];
                    sampleCount++;
                }
            }
        }

        avgHeight /= sampleCount;

        avgHeight = Mathf.Round(avgHeight * 10f) / 10f;

        for (int x = centerX - 1; x <= centerX + 1; x++)
        {
            for (int z = centerZ - 1; z <= centerZ + 1; z++)
            {
                if (x >= 0 && x < mapWidth && z >= 0 && z < mapHeight)
                {
                    heightMap[x, z] = avgHeight;
                    if (grid[x, z] != null && grid[x, z].currentTileObject != null)
                    {
                        Vector3 newPos = grid[x, z].currentTileObject.transform.position;
                        newPos.y = avgHeight;
                        grid[x, z].currentTileObject.transform.position = newPos;
                    }
                }
            }
        }

        return new Vector3(
            centerX * tileSize,
            avgHeight,
            centerZ * tileSize
        );
    }

    void RebuildNavMesh()
    {
        NavMeshSurface oldSurface = GetComponent<NavMeshSurface>();
        if (oldSurface != null)
        {
            DestroyImmediate(oldSurface);
        }

        NavMeshSurface surface = gameObject.AddComponent<NavMeshSurface>();
        surface.useGeometry = NavMeshCollectGeometry.PhysicsColliders;
        surface.agentTypeID = 0;
        surface.overrideTileSize = true;
        surface.overrideVoxelSize = true;
        surface.voxelSize = 0.2f;
        surface.tileSize = 32;
        surface.BuildNavMesh();
    }

    public TileData GetTileAtWorldPosition(Vector3 worldPosition)
    {
        if (grid == null) return null;

        int x = Mathf.FloorToInt(worldPosition.x / tileSize);
        int y = Mathf.FloorToInt(worldPosition.z / tileSize);

        if (x >= 0 && x < mapWidth && y >= 0 && y < mapHeight)
            return grid[x, y];

        return null;
    }
}

