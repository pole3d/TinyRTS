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
    private HashSet<Vector3Int> _toAdd = new HashSet<Vector3Int>();

    private List<Vector3Int> _addTiles = new List<Vector3Int>();
    private List<Vector3Int> _removeTiles = new List<Vector3Int>();
    private List<Viewers> _allViewers = new List<Viewers>();


    private int _fogRadiusBase = 8;
    private int[,] _tileStatus;
    private bool[,] _tileActivatedCurrent;
    private bool[,] _tileActivatedOld;
    private TileBase[] _tileShadow;
    private Vector2Int _offsetCell;
    private Vector2Int _mapSize;
    private Vector3Int[] _allMap;

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

        Viewers viewer = new Viewers(newViewer, new HashSet<Vector2Int>(), fogRadius);
        _discoveredTiles.Add(viewer);
        _allViewers.Add(viewer);
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

        _allMap = new Vector3Int[_mapSize.x * _mapSize.y];

        BoundsInt bounds = _underTilemap.cellBounds;
        foreach (var position in bounds.allPositionsWithin)
        {
            _fogOfWarTilemap.SetTile(position, _undiscoveredTile);
        }

        _mapSize.x = _fogOfWarTilemap.size.x;
        _mapSize.y = _fogOfWarTilemap.size.y;

        _tileStatus = new int[_mapSize.x, _mapSize.y];
        _tileActivatedCurrent = new bool[_mapSize.x, _mapSize.y];
        _tileActivatedOld = new bool[_mapSize.x, _mapSize.y];
        _tileShadow = new TileBase[_mapSize.x * _mapSize.y];

        for (int i = 0; i < _tileShadow.Length; i++)
        {
            _tileShadow[i] = _discoveredTile;
        }

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
            // CheckDiscoveredTiles(viewer);
            var discoveredViewer = GetGoodViewers(viewer);
            //AddDiscoveredTiles(viewer, discoveredViewer);
        }

        ClearBuffer();
        WriteViewer();
        CompareToOldList();
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

                //var discoveredViewer = GetGoodViewers(_viewersTransforms[i]);
                //AddDiscoveredTiles(_viewersTransforms[i], discoveredViewer);
                // ClearBuffer();
                // WriteViewer();
                // CompareToOldList();
            }
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            ClearBuffer();
            WriteViewer();
            CompareToOldList();
        }
    }

    private void ClearBuffer()
    {
        for (int x = _mapSize.x; x < 0; x++)
        {
            for (int y = _mapSize.y; y < 0; y++)
            {
                _tileActivatedCurrent[x, y] = false;
            }
        }
    }

    private void WriteViewer()
    {
        foreach (var viewer in _viewersTransforms)
        {
            var viewerTilePos = new Vector2Int((int)viewer.position.x, (int)viewer.position.y);
            // print(viewerTilePos);
            var fogRadius = GetGoodViewers(viewer).FogRadius;
            var fogRadiusSq = fogRadius * fogRadius;

            // For all tiles in the radius
            for (int x = viewerTilePos.x - fogRadius; x <= viewerTilePos.x + fogRadius; x++)
            {
                for (int y = viewerTilePos.y - fogRadius; y <= viewerTilePos.y + fogRadius; y++)
                {
                    Vector2Int cellPosition = new Vector2Int(x, y);
                    Vector2Int direction = viewerTilePos - cellPosition;

                    var cellPos = GetTileStatusPos(cellPosition.x, cellPosition.y);

                    // print($"cellPos : {cellPos}");
                    // Check if it is inside the FogRadius
                    if (direction.sqrMagnitude <= fogRadiusSq)
                    {
                        print("go activated");
                        _tileActivatedCurrent[cellPos.x, cellPos.y] = true;
                    }
                }
            }
        }
    }

    private void CompareToOldList()
    {
        for (int x = 0; x < _mapSize.x; x++)
        {
            for (int y = 0; y < _mapSize.y; y++)
            {
                // print($"current : {_tileActivatedCurrent[x,y]} / old : {_tileActivatedOld[x,y]}");
                if (_tileActivatedOld[x, y] == true && _tileActivatedCurrent[x, y] == false)
                {
                    _removeTiles.Add(GetWorldPosTile(x,y));
                    _fogOfWarTilemap.SetTile(GetWorldPosTile(x,y), _discoveredTile);
                }
                else if (_tileActivatedOld[x, y] == false && _tileActivatedCurrent[x, y] == true)
                {
                    _addTiles.Add(GetWorldPosTile(x,y));
                    _fogOfWarTilemap.SetTile(GetWorldPosTile(x,y), null);
                }
            }
        }

        print($"_removeTiles : {_removeTiles.Count}");
        print($"_addTiles : {_addTiles.Count}");

        TileBase[] removeAdd = new TileBase[_removeTiles.Count + _addTiles.Count];
        Vector3Int[] removeAddPos = new Vector3Int[_removeTiles.Count + _addTiles.Count];

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
        //         // print($"add : {_addTiles[i]}");
        //         removeAddPos[i] = _addTiles[i];
        //     }
        // }
        //
        // _fogOfWarTilemap.SetTiles(removeAddPos, removeAdd);
        // print($"removeAddPos : {removeAddPos.Length} removeAdd : {removeAdd.Length}");
        
        _removeTiles.Clear();
        _addTiles.Clear();

        (_tileActivatedOld, _tileActivatedCurrent) = (_tileActivatedCurrent, _tileActivatedOld);
    }


    /// <summary>
    /// Checks and updates discovered tiles based on viewer movement
    /// </summary>
    private void CheckDiscoveredTiles(Transform viewer)
    {
        var viewerTilePos = new Vector2Int((int)viewer.position.x, (int)viewer.position.y);

        // Check for discovered tiles if they are outside the FogRadius range
        var discoveredViewer = GetGoodViewers(viewer);

        foreach (var discoveredTile in discoveredViewer.DiscoveredTiles)
        {
            if (Vector2Int.Distance(viewerTilePos, discoveredTile) > discoveredViewer.FogRadius)
            {
                // Change these tiles with discoveredTile and remove them from the discoveredViewer.DiscoveredTiles list 
                Vector2Int cellPos = GetTileStatusPos(discoveredTile.x, discoveredTile.y);

                if (!CheckIfInsideOfMap(cellPos))
                    continue;

                _tileStatus[cellPos.x, cellPos.y]--;
                // print($"reduce {_tileStatus[cellPos.x, cellPos.y]}");

                // Check if status of the tile with no viewer and add the shadow tile
                if (_tileStatus[cellPos.x, cellPos.y] <= 0)
                {
                    _tileStatus[cellPos.x, cellPos.y] = 0;
                    _toAdd.Add(new Vector3Int(discoveredTile.x, discoveredTile.y, 0));
                }
            }
        }

        // Remove from the tile of the viewer's current list
        TileBase[] tilesDiscovered = new TileBase[_toAdd.Count];
        _fogOfWarTilemap.SetTiles(_toAdd.ToArray(), tilesDiscovered);

        _toAdd.Clear();
        discoveredViewer.DiscoveredTiles.Clear();

        AddDiscoveredTiles(viewer, discoveredViewer);
    }


    /// <summary>
    /// Adds newly discovered tiles to the Fog of War
    /// </summary>
    private void AddDiscoveredTiles(Transform viewer, Viewers discoveredViewer)
    {
        var viewerTilePos = new Vector2Int((int)viewer.position.x, (int)viewer.position.y);
        var fogRadius = discoveredViewer.FogRadius;
        var fogRadiusSq = fogRadius * fogRadius;

        // For all tiles in the radius
        for (int x = viewerTilePos.x - fogRadius; x <= viewerTilePos.x + fogRadius; x++)
        {
            for (int y = viewerTilePos.y - fogRadius; y <= viewerTilePos.y + fogRadius; y++)
            {
                Vector2Int cellPosition = new Vector2Int(x, y);
                Vector2Int direction = viewerTilePos - cellPosition;

                // Check if it is inside the FogRadius
                if (direction.sqrMagnitude <=
                    fogRadiusSq) // && !discoveredViewer.DiscoveredTiles.Contains(cellPosition))
                {
                    _fogOfWarTilemap.SetTile(new Vector3Int(cellPosition.x, cellPosition.y, 0), null);
                    // discoveredViewer.DiscoveredTiles.Add(cellPosition);

                    _toAdd.Add(new Vector3Int(cellPosition.x, cellPosition.y, 0));

                    Vector2Int cellPos = GetTileStatusPos(cellPosition.x, cellPosition.y);
                    // Add 1 to the status of this tile
                    if (CheckIfInsideOfMap(cellPos))
                        _tileStatus[cellPos.x, cellPos.y]++;

                    // print($"add : {_tileStatus[cellPos.x, cellPos.y]}");
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
    /// Returns the Tile Status Position from Cell World Position  
    /// </summary>
    private Vector2Int GetTileStatusPos(int x, int y)
    {
        int posX = x + _offsetCell.x;
        int posY = y + _offsetCell.y;
        return new Vector2Int(posX, posY);
    }

    private Vector3Int GetWorldPosTile(int x, int y)
    {
        int posX = x - _offsetCell.x;
        int posY = y - _offsetCell.y;
        return new Vector3Int(posX, posY,0);
    }

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
    public readonly HashSet<Vector2Int> DiscoveredTiles;
    public readonly int FogRadius;

    public Viewers(Transform transform, HashSet<Vector2Int> discoveredTiles, int fogRadius)
    {
        Transform = transform;
        DiscoveredTiles = discoveredTiles;
        FogRadius = fogRadius;
    }
}