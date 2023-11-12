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

    private List<Viewers> _discoveredViewers = new List<Viewers>();

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

            Viewers newViewer = new Viewers(viewer, new List<Vector2Int>(), _fogRadius);
            _discoveredViewers.Add(newViewer);
            _fogRadius++;
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

        // Check for discovered tiles if they are outside the FogRadius range
        var discoveredViewer = GetGoodViewers(viewer);

        foreach (var discoveredTile in discoveredViewer.DiscoveredTiles)
        {
            if (Vector2Int.Distance(viewerTilePos, discoveredTile) > discoveredViewer.FogRadius)
            {
                tilesToRemove.Add(discoveredTile);
            }
        }

        // Change these tiles with discoveredTile and remove them from the _discoveredTilesList 
        foreach (Vector2Int discoveredTile in tilesToRemove)
        {
            _fogOfWarTilemap.SetTile(new Vector3Int(discoveredTile.x, discoveredTile.y, 0), _discoveredTile);
            discoveredViewer.DiscoveredTiles.Remove(discoveredTile);
        }

        AddDiscoveredTiles(viewer, discoveredViewer);
    }

    private void AddDiscoveredTiles(Transform viewer, Viewers discoveredViewer)
    {
        var viewerTilePos = new Vector2Int((int)viewer.position.x, (int)viewer.position.y);
        var FogRadius = discoveredViewer.FogRadius;

        // For all tiles in the radius
        for (int x = viewerTilePos.x - FogRadius; x <= viewerTilePos.x + FogRadius; x++)
        {
            for (int y = viewerTilePos.y - FogRadius; y <= viewerTilePos.y + FogRadius; y++)
            {
                Vector2Int cellPosition = new Vector2Int(x, y);

                // Check if it is inside the FogRadius and not clear to avoid remove visibility to other viewers
                var isNotShadow = _fogOfWarTilemap.GetTile(new Vector3Int(cellPosition.x, cellPosition.y, 0));

                if (Vector2Int.Distance(viewerTilePos, cellPosition) <= FogRadius && isNotShadow)
                {
                    _fogOfWarTilemap.SetTile(new Vector3Int(cellPosition.x, cellPosition.y, 0), null);
                    discoveredViewer.DiscoveredTiles.Add(cellPosition);
                }
            }
        }
    }

    private Viewers GetGoodViewers(Transform viewer)
    {
        foreach (var discoViewer in _discoveredViewers)
        {
            if (viewer == discoViewer.Transform)
            {
                return discoViewer;
            }
        }

        return null;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        foreach (var viewer in _discoveredViewers)
        {
            Gizmos.DrawWireSphere(viewer.Transform.position, viewer.FogRadius);
        }
    }
}

public class Viewers
{
    public readonly Transform Transform;
    public readonly List<Vector2Int> DiscoveredTiles;
    public readonly int FogRadius;

    public Viewers(Transform transform, List<Vector2Int> discoveredTiles, int fogRadius)
    {
        Transform = transform;
        DiscoveredTiles = discoveredTiles;
        FogRadius = fogRadius;
    }
}