using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

public class FogOfWar : MonoBehaviour
{
    [Header("-- Tilemap --")] [SerializeField]
    private Tilemap _fogOfWarTilemap;

    [SerializeField] private Tilemap _underTilemap;

    [Header("-- Tile --")] [SerializeField]
    private TileBase _discoveredTile;

    [SerializeField] private TileBase _undiscoveredTile;

    [Header("-- Settings --")] [SerializeField]
    private Transform[] _viewers;

    [Range(1, 15)] [SerializeField] private int _fogRadius = 8;

    private List<Vector3> _lastPlayerPosition = new List<Vector3>();

    private readonly HashSet<Vector3Int> _discoveredTilesList = new HashSet<Vector3Int>();

    private const float MinDistancePlayerMove = 0.01f;


    private void Start()
    {
        PaintAllFogOfWarTilemap();

        AddAllViewers();

       StartDiscoveredTiles();
    }

    private void AddAllViewers()
    {
        foreach (var viewer in _viewers)
        {
            _lastPlayerPosition.Add(viewer.position);
        }
    }

    private void PaintAllFogOfWarTilemap()
    {
        BoundsInt bounds = _underTilemap.cellBounds;
        foreach (var position in bounds.allPositionsWithin)
        {
            _fogOfWarTilemap.SetTile(position, _undiscoveredTile);
        }
    }

    private void StartDiscoveredTiles()
    {
        foreach (var viewer in _viewers)
        {
            Vector3Int viewerTilePos = _fogOfWarTilemap.WorldToCell(viewer.position);

            CheckDiscoveredTiles(viewerTilePos);
        }
    }

    private void Update()
    {
        CheckIfViewersMove();
    }

    private void CheckIfViewersMove()
    {
        for (int i = 0; i < _viewers.Length; i++)
        {
            if (Vector3.Distance(_lastPlayerPosition[i], _viewers[i].position) > MinDistancePlayerMove)
            {
                Vector3Int viewerTilePos = _fogOfWarTilemap.WorldToCell(_viewers[i].position);

                CheckDiscoveredTiles(viewerTilePos);

                _lastPlayerPosition[i] = _viewers[i].position;
            }
        }
    }

    private void CheckDiscoveredTiles(Vector3Int viewerTilePos)
    {
        List<Vector3Int> tilesToRemove = new List<Vector3Int>();

        // Check for discovered tiles if they are outside the _fogRadius range
        foreach (Vector3Int discoveredTile in _discoveredTilesList)
        {
            if (Vector3Int.Distance(viewerTilePos, discoveredTile) > _fogRadius)
            {
                tilesToRemove.Add(discoveredTile);
            }
        }

        // Change these tiles with discoveredTile and remove them from the _discoveredTilesList 
        foreach (Vector3Int discoveredTile in tilesToRemove)
        {
            _fogOfWarTilemap.SetTile(discoveredTile, _discoveredTile);
            _discoveredTilesList.Remove(discoveredTile);
        }

        AddDiscoveredTiles(viewerTilePos);
    }

    private void AddDiscoveredTiles(Vector3Int viewerTilePos)
    {
        // For all tiles in the radius
        for (int x = viewerTilePos.x - _fogRadius; x <= viewerTilePos.x + _fogRadius; x++)
        {
            for (int y = viewerTilePos.y - _fogRadius; y <= viewerTilePos.y + _fogRadius; y++)
            {
                Vector3Int cellPosition = new Vector3Int(x, y, 0);

                // Check if it is inside the _fogRadius
                if (Vector3Int.Distance(viewerTilePos, cellPosition) <= _fogRadius)
                {
                    _fogOfWarTilemap.SetTile(cellPosition, null);
                    _discoveredTilesList.Add(cellPosition);
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        foreach (var viewer in _viewers)
        {
            Gizmos.DrawWireSphere(viewer.position, _fogRadius);
        }
    }
}