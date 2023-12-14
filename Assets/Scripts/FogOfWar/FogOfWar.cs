using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

/// <summary>
/// Manages the Fog of War in the game
/// </summary>
public class FogOfWar : MonoBehaviour
{
    [Header("-- Tilemap --")] [SerializeField]
    private Tilemap _fogOfWarTilemap;

    [SerializeField] private Tilemap _underTilemap;

    [Header("-- Tile --")] [SerializeField]
    private TileBase _discoveredTile;

    [SerializeField] private TileBase _undiscoveredTile;

    [Header("-- Settings --")] [SerializeField]
    private List<Transform> _startViewers = new List<Transform>();


    private List<Transform> _viewersTransforms = new List<Transform>();
    private List<Viewers> _viewers;
    private List<Vector3> _lastPlayerPosition = new List<Vector3>();
    private List<Viewers> _discoveredTiles = new List<Viewers>();

    private int _fogRadiusBase = 8;
    private int[,] _tileStatus;
    private Vector2Int _offsetCell;


    private const float MinDistancePlayerMove = 0.01f;

    private void Start()
    {
        Initialize();
    }

    /// <summary>
    /// Initializes the Fog of War
    /// </summary>
    public void Initialize()
    {
        PaintAllFogOfWarTilemap();
        AddStartViewers();
        StartDiscoveredTiles();
    }

    /// <summary>
    /// Add new viewer to the Fog of War system
    /// </summary>
    public void AddNewViewer(Transform newViewer, int fogRadius)
    {
        _viewersTransforms.Add(newViewer);

        Vector3 pos = newViewer.position;
        _lastPlayerPosition.Add(pos);

        if (fogRadius == 0)
        {
            fogRadius = _fogRadiusBase;
        }

        Viewers viewer = new Viewers(newViewer, new List<Vector2Int>(), fogRadius);
        _discoveredTiles.Add(viewer);
    }

    /// <summary>
    /// Adds start viewers to the Fog of War system
    /// </summary>
    private void AddStartViewers()
    {
        foreach (var viewer in _startViewers)
        {
            AddNewViewer(viewer, _fogRadiusBase);
        }
    }

    /// <summary>
    /// Paints the entire Fog of War tilemap with the undiscovered tile
    /// </summary>
    private void PaintAllFogOfWarTilemap()
    {
        if (_underTilemap == null)
        {
            Debug.LogWarning("Under TileMap in FogOfWar not found");
            return;
        }

        BoundsInt bounds = _underTilemap.cellBounds;
        foreach (var position in bounds.allPositionsWithin)
        {
            _fogOfWarTilemap.SetTile(position, _undiscoveredTile);
        }

        _tileStatus = new int[_fogOfWarTilemap.size.x, _fogOfWarTilemap.size.y];

        // Setup offset of the tileMap for the tileStatus
        _offsetCell.x = Math.Abs(_fogOfWarTilemap.origin.x);
        _offsetCell.y = Math.Abs(_fogOfWarTilemap.origin.y);
    }

    /// <summary>
    /// Initiates the discovery of tiles by viewers
    /// </summary>
    private void StartDiscoveredTiles()
    {
        foreach (var viewer in _viewersTransforms)
        {
            CheckDiscoveredTiles(viewer);
        }
    }

    /// <summary>
    /// Update is called once per frame, checks if viewers have moved
    /// </summary>
    private void Update()
    {
        CheckIfViewersMove();
    }

    /// <summary>
    /// Checks if viewers have moved and updates discovered tiles accordingly
    /// </summary>
    private void CheckIfViewersMove()
    {
        for (int i = 0; i < _viewersTransforms.Count; i++)
        {
            if (Vector2.Distance(_lastPlayerPosition[i], _viewersTransforms[i].position) > MinDistancePlayerMove)
            {
                _lastPlayerPosition[i] = _viewersTransforms[i].position;
                CheckDiscoveredTiles(_viewersTransforms[i]);
            }
        }
    }

    /// <summary>
    /// Checks and updates discovered tiles based on viewer movement
    /// </summary>
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

        // Change these tiles with discoveredTile and remove them from the discoveredViewer.DiscoveredTiles list 
        foreach (Vector2Int tileToRemove in tilesToRemove)
        {
            int cellPosX = tileToRemove.x + _offsetCell.x;
            int cellPosY = tileToRemove.y + _offsetCell.y;
            _tileStatus[cellPosX, cellPosY]--;
            
            // Check if status of the tile with no viewer and add the shadow tile
            if (_tileStatus[cellPosX, cellPosY] <= 0)
            {
                _tileStatus[cellPosX, cellPosY] = 0;
                _fogOfWarTilemap.SetTile(new Vector3Int(tileToRemove.x, tileToRemove.y, 0), _discoveredTile);
            }

            // Remove from the tile of the viewer's current list
            discoveredViewer.DiscoveredTiles.Remove(tileToRemove);
        }

        AddDiscoveredTiles(viewer, discoveredViewer);
    }

    /// <summary>
    /// Adds newly discovered tiles to the Fog of War
    /// </summary>
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

                // Check if it is inside the FogRadius
                if (Vector2Int.Distance(viewerTilePos, cellPosition) <= FogRadius)
                {
                    _fogOfWarTilemap.SetTile(new Vector3Int(cellPosition.x, cellPosition.y, 0), null);
                    discoveredViewer.DiscoveredTiles.Add(cellPosition);

                    int cellPosX = cellPosition.x + _offsetCell.x;
                    int cellPosY = cellPosition.y + _offsetCell.y;
                    // Add 1 to the status of this tile
                    _tileStatus[cellPosX, cellPosY]++;
                }
            }
        }
    }

    /// <summary>
    /// Returns the Viewers object associated with the given viewer transform
    /// </summary>
    private Viewers GetGoodViewers(Transform viewer)
    {
        foreach (var discoViewer in _discoveredTiles)
        {
            if (viewer == discoViewer.Transform)
            {
                return discoViewer;
            }
        }

        return null;
    }

    /// <summary>
    /// Draws wire spheres around viewers in the scene for visualization
    /// </summary>
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        foreach (var viewer in _discoveredTiles)
        {
            Gizmos.DrawWireSphere(viewer.Transform.position, viewer.FogRadius);
        }
    }
}

/// <summary>
/// Represents a viewer in the Fog of War system
/// </summary>
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