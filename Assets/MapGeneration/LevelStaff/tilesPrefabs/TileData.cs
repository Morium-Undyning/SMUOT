using UnityEngine;
using System;

public enum TileType
{
    Grass,
    Dirt,
    Stone
}

[Serializable]
public class TileData
{
    public Vector2Int gridPosition;
    public TileType type;
    public GameObject currentTileObject;
    public bool isWalkable = true;
    public bool isBuildable = true;
    public float height = 0f;
    public float noiseValue;
    public float GetHeight() => height;
}

