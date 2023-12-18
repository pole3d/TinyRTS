using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// Manages the Fog of War in the game
/// </summary>
public class FogOfWar : MonoBehaviour
{
    [Header("-- Tilemap --")]
    [SerializeField] private Tilemap _fogOfWarTilemap;
    [SerializeField] private Tilemap _underTilemap;

    [Header("-- Tile --")]
    [SerializeField] private TileBase _discoveredTile;
    [SerializeField] private TileBase _undiscoveredTile;

    [Header("-- Settings --")]
    [SerializeField] private List<Transform> _startViewers = new List<Transform>();
    
    
    private List<Transform> _viewersTransforms = new List<Transform>();
    private List<Viewers> _allViewers = new List<Viewers>();

    // Keep it if needed to switch to SetTiles despite of SetTile
    private List<Vector3Int> _addTiles = new List<Vector3Int>();
    private List<Vector3Int> _removeTiles = new List<Vector3Int>();

    private int _fogRadiusBase = 8;
    private int _indexViewer = 0;
    private int _calculateViewerByFrame = 0;
    
    private bool[,] _tileActivatedCurrent;
    private bool[,] _tileActivatedOld;
    
    private Vector2Int _offsetCell;
    private Vector2Int _mapSize;

    private const int Max_Viewer_Calculated_By_Frame = 5;
    
    
    /// <summary>
    /// Initializes the Fog of War
    /// Paint the map with shadow
    /// Add Viewers from the inspector
    /// Clear the Current boolean Map
    /// Setup variables
    /// </summary>
    public void Initialize()
    {
        PaintAllFogOfWarTilemap();
        AddStartViewers();
        ClearBuffer();
        SetupMapSizeAndOffset();
    }

    /// <summary>
    /// Add new viewer to the Fog of War system
    /// </summary>
    public void AddNewViewer(Transform newViewer, int fogRadius)
    {
        _viewersTransforms.Add(newViewer);

        if (fogRadius == 0)
        {
            fogRadius = _fogRadiusBase;
        }

        Viewers viewer = new Viewers(newViewer, new HashSet<Vector2Int>(), fogRadius);
        _allViewers.Add(viewer);
        
        // Init the good number of viewer calculated by frame
        _calculateViewerByFrame = _viewersTransforms.Count < Max_Viewer_Calculated_By_Frame ? _viewersTransforms.Count : Max_Viewer_Calculated_By_Frame;
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
    }

    /// <summary>
    /// Setup the variables
    /// </summary>
    private void SetupMapSizeAndOffset()
    {
        _mapSize.x = _fogOfWarTilemap.size.x;
        _mapSize.y = _fogOfWarTilemap.size.y;

        _tileActivatedCurrent = new bool[_mapSize.x, _mapSize.y];
        _tileActivatedOld = new bool[_mapSize.x, _mapSize.y];

        // Setup offset of the tileMap for the tileStatus
        _offsetCell.x = Math.Abs(_fogOfWarTilemap.origin.x);
        _offsetCell.y = Math.Abs(_fogOfWarTilemap.origin.y);
    }

    private void Update()
    {
        WriteViewer();
    }

    /// <summary>
    /// Clean the current map with activated tile
    /// </summary>
    private void ClearBuffer()
    {
        for (int x = 0; x < _mapSize.x; x++)
        {
            for (int y = 0; y < _mapSize.y; y++)
            {
                _tileActivatedCurrent[x, y] = false;
            }
        }
    }

    /// <summary>
    /// Depending the radius of the viewer, it adds tiles in range to the _tileActivatedCurrent by true
    /// If it has calculated every viewer, it refresh the map, clear the current map and restart with the first viewer
    /// </summary>
    private void WriteViewer()
    {
        for (int i = 0; i < _calculateViewerByFrame; i++)
        {
            if (i + _indexViewer >= _viewersTransforms.Count)
            {
                CompareToOldList();
                _indexViewer = 0;
                ClearBuffer();
            }
            
            var viewer = _viewersTransforms[i + _indexViewer];
            var position = viewer.position;
            var viewerTilePos = new Vector2Int((int)position.x, (int)position.y);
            var fogRadius = GetGoodViewers(viewer).FogRadius;
            var fogRadiusSq = fogRadius * fogRadius;

            // For all tiles in the radius
            int count = 0;
            for (int x = viewerTilePos.x - fogRadius; x <= viewerTilePos.x + fogRadius; x++)
            {
                for (int y = viewerTilePos.y - fogRadius; y <= viewerTilePos.y + fogRadius; y++)
                {
                    Vector2Int cellPosition = new Vector2Int(x, y);
                    Vector2Int direction = viewerTilePos - cellPosition;

                    var cellPos = GetTileStatusPos(cellPosition.x, cellPosition.y);

                    // Check if it is inside the FogRadius
                    if (direction.sqrMagnitude <= fogRadiusSq)
                    {
                        if(!CheckIfInsideOfMap(new Vector2Int(cellPos.x,cellPos.y)))
                            continue;
                        
                        count++;
                        _tileActivatedCurrent[cellPos.x, cellPos.y] = true;
                    }
                }
            }
        }

        _indexViewer += _calculateViewerByFrame;
    }

    /// <summary>
    /// Compare the Current boolean map with the old and apply the differences
    /// Use SetTile because less consuming than SetTiles but keep SetTiles if needed to reimplement 
    /// </summary>
    private void CompareToOldList()
    {
        for (int x = 0; x < _mapSize.x; x++)
        {
            for (int y = 0; y < _mapSize.y; y++)
            {
                if (_tileActivatedOld[x, y] == true && _tileActivatedCurrent[x, y] == false)
                {
                    // _removeTiles.Add(GetWorldPosTile(x,y));
                    _tileActivatedOld[x, y] = false;

                    _fogOfWarTilemap.SetTile(GetWorldPosTile(x,y), _discoveredTile);
                }
                else if (_tileActivatedOld[x, y] == false && _tileActivatedCurrent[x, y] == true)
                {
                    // _addTiles.Add(GetWorldPosTile(x,y));
                    _tileActivatedOld[x, y] = true;

                    _fogOfWarTilemap.SetTile(GetWorldPosTile(x,y), null);
                }
            }
        }

        // If SetTile not optimize enough -> actually better than SetTiles
        // TileBase[] removeAdd = new TileBase[_removeTiles.Count + _addTiles.Count];
        // Vector3Int[] removeAddPos = new Vector3Int[_removeTiles.Count + _addTiles.Count];
        //
        //
        // for (int i = 0; i < removeAdd.Length; i++)
        // {
        //     if (i < _removeTiles.Count)
        //     {
        //         removeAdd[i] = _discoveredTile;
        //         removeAddPos[i] = _removeTiles[i];
        //     }
        //     else
        //     {
        //         removeAdd[i] = null;
        //         removeAddPos[i] = _addTiles[i-_removeTiles.Count];
        //     }
        // }
        // _fogOfWarTilemap.SetTiles(removeAddPos, removeAdd);
        //
        // _removeTiles.Clear();
        // _addTiles.Clear();
    }
    
    
    /// <summary>
    /// Returns the Viewers object associated with the given viewer transform
    /// </summary>
    private Viewers GetGoodViewers(Transform viewer)
    {
        foreach (var discoViewer in _allViewers)
        {
            if (viewer == discoViewer.Transform)
            {
                return discoViewer;
            }
        }

        return null;
    }

    /// <summary>
    /// Returns the Tile Status Position from Cell World Position  
    /// </summary>
    private Vector2Int GetTileStatusPos(int x, int y)
    {
        int posX = x + _offsetCell.x;
        int posY = y + _offsetCell.y;
        return new Vector2Int(posX, posY);
    }
    
    /// <summary>
    /// Returns the World Position from Cell Position  
    /// </summary>
    private Vector3Int GetWorldPosTile(int x, int y)
    {
        int posX = x - _offsetCell.x;
        int posY = y - _offsetCell.y;
        return new Vector3Int(posX, posY,0);
    }

    /// <summary>
    /// Returns if coordinate are in range of mapSize  
    /// </summary>
    private bool CheckIfInsideOfMap(Vector2Int pos)
    {
        if (pos.x < 0 || pos.y < 0 || pos.x >= _mapSize.x || pos.y >= _mapSize.y)
            return false;

        return true;
    }

    /// <summary>
    /// Draws wire spheres around viewers in the scene for visualization
    /// </summary>
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        foreach (var viewer in _allViewers)
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
    public readonly HashSet<Vector2Int> DiscoveredTiles;
    public readonly int FogRadius;

    public Viewers(Transform transform, HashSet<Vector2Int> discoveredTiles, int fogRadius)
    {
        Transform = transform;
        DiscoveredTiles = discoveredTiles;
        FogRadius = fogRadius;
    }
}