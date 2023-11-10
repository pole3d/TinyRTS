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
    private Transform[] _viewerTransforms;
    private Viewers[] _viewers;

    [Range(1, 15)] [SerializeField] private int _fogRadius = 8;

    private List<Vector3> _lastPlayerPosition = new List<Vector3>();

    private Dictionary<Transform, List<Vector2Int>> _discoveredTileByViewers = new Dictionary<Transform, List<Vector2Int>>();

    private const float MinDistancePlayerMove = 0.01f;


    private void Start()
    {
        PaintAllFogOfWarTilemap();

        AddAllViewers();

       StartDiscoveredTiles();
    }

    private void AddAllViewers()
    {
        foreach (var viewer in _viewerTransforms)
        {
            var position = viewer.position;
            
            _lastPlayerPosition.Add(position);
            
            _discoveredTileByViewers.Add(viewer, new List<Vector2Int>());
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
        for (int i = 0; i < _viewerTransforms.Length; i++)
        {
            Vector2Int viewerTilePos = GetViewerTilePos(i);

            CheckDiscoveredTiles(_viewerTransforms[i]);
        }
    }

    private Vector2Int GetViewerTilePos(int i)
    {
        Vector3Int viewerTilePos3 = _fogOfWarTilemap.WorldToCell(_viewerTransforms[i].position);
        return new Vector2Int(viewerTilePos3.x, viewerTilePos3.y);
    }

    private void Update()
    {
        CheckIfViewersMove();
    }

    private void CheckIfViewersMove()
    {
        for (int i = 0; i < _viewerTransforms.Length; i++)
        {
            if (Vector2.Distance(_lastPlayerPosition[i], _viewerTransforms[i].position) > MinDistancePlayerMove)
            {
                CheckDiscoveredTiles(_viewerTransforms[i]);

                _lastPlayerPosition[i] = _viewerTransforms[i].position;
            }
        }
    }

    private void CheckDiscoveredTiles(Transform viewer)
    {
        var viewerTilePos = new Vector2Int((int)viewer.position.x, (int)viewer.position.y);
        List<Vector2Int> tilesToRemove = new List<Vector2Int>();

        // Check for discovered tiles if they are outside the _fogRadius range
        foreach (Vector2Int discoveredTile in _discoveredTileByViewers[viewer])
        {
            if (Vector2Int.Distance(viewerTilePos, discoveredTile) > _fogRadius)
            {
                tilesToRemove.Add(discoveredTile);
            }
        }

        // Change these tiles with discoveredTile and remove them from the _discoveredTilesList 
        foreach (Vector2Int discoveredTile in tilesToRemove)
        {
            _fogOfWarTilemap.SetTile(new Vector3Int(discoveredTile.x, discoveredTile.y, 0), _discoveredTile);
            _discoveredTileByViewers[viewer].Remove(discoveredTile);
            // _discoveredTilesList.Remove(discoveredTile);
        }

        AddDiscoveredTiles(viewer);
    }

    private void AddDiscoveredTiles(Transform viewer)
    {
        var viewerTilePos = new Vector2Int((int)viewer.position.x, (int)viewer.position.y);

        // For all tiles in the radius
        for (int x = viewerTilePos.x - _fogRadius; x <= viewerTilePos.x + _fogRadius; x++)
        {
            for (int y = viewerTilePos.y - _fogRadius; y <= viewerTilePos.y + _fogRadius; y++)
            {
                Vector2Int cellPosition = new Vector2Int(x, y);

                // Check if it is inside the _fogRadius
                if (Vector2Int.Distance(viewerTilePos, cellPosition) <= _fogRadius)
                {
                    _fogOfWarTilemap.SetTile(new Vector3Int(cellPosition.x, cellPosition.y, 0), null);
                    // _discoveredTilesList.Add(cellPosition);
                    _discoveredTileByViewers[viewer].Add(cellPosition);
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        foreach (var viewer in _viewerTransforms)
        {
            Gizmos.DrawWireSphere(viewer.position, _fogRadius);
        }
    }
}

public class Viewers
{
    public Transform Position;
    public List<Vector2Int> DiscoveredTile;
}