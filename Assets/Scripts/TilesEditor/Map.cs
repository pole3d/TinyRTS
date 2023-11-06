using System.Collections.Generic;
using System.IO;
using TilesEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Map : MonoBehaviour
{
    [field: SerializeField] public Vector2Int MapSize { get; private set; }
    [field: SerializeField] public Grid Grid { get; private set; }

    [SerializeField] private Tile _defaultTile;
    [SerializeField] private Tilemap _defaultTilemap;

    private void Start()
    {
        FillMap();
    }

    /// <summary>
    /// Create a map with the given size and the default tile, on the given tilemap.
    /// </summary>
    private void FillMap()
    {
        for (int x = 0; x < MapSize.x; x++)
        {
            for (int y = 0; y < MapSize.y; y++)
            {
                _defaultTilemap.SetTile(new Vector3Int(x, y, 0), _defaultTile);
            }
        }
    }

    /// <summary>
    /// Add a tile at a position on the tilemap.
    /// </summary>
    /// <param name="tile"> Tile to add. </param>
    /// <param name="position"> Position to add the tile. </param>
    public void AddTile(Tile tile, Vector3Int position)
    {
        _defaultTilemap.SetTile(position, tile);
    }
}