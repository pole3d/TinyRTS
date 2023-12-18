using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
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
        StartCoroutine(WaitToUpdateFogOfWar());
    }

    IEnumerator WaitToUpdateFogOfWar()
    {
        ClearBuffer();
        WriteViewer();
        CompareToOldList();
        yield return new WaitForSeconds(.5f);
        StartCoroutine(WaitToUpdateFogOfWar());
    }

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

    private void WriteViewer()
    {
        foreach (var viewer in _viewersTransforms)
        {
            var viewerTilePos = new Vector2Int((int)viewer.position.x, (int)viewer.position.y);
            // print(viewerTilePos);
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
    }

    private void CompareToOldList()
    {
        for (int x = 0; x < _mapSize.x; x++)
        {
            for (int y = 0; y < _mapSize.y; y++)
            {
                if (_tileActivatedOld[x, y] == true && _tileActivatedCurrent[x, y] == false)
                {
                    _removeTiles.Add(GetWorldPosTile(x,y));
                    _tileActivatedOld[x, y] = false;

                    _fogOfWarTilemap.SetTile(GetWorldPosTile(x,y), _discoveredTile);
                }
                else if (_tileActivatedOld[x, y] == false && _tileActivatedCurrent[x, y] == true)
                {
                    _addTiles.Add(GetWorldPosTile(x,y));
                    _tileActivatedOld[x, y] = true;

                    _fogOfWarTilemap.SetTile(GetWorldPosTile(x,y), null);
                }
            }
        }

        // print($"_removeTiles : {_removeTiles.Count}");
        // print($"_addTiles : {_addTiles.Count}");

        // TileBase[] removeAdd = new TileBase[_removeTiles.Count + _addTiles.Count];
        // Vector3Int[] removeAddPos = new Vector3Int[_removeTiles.Count + _addTiles.Count];
        
        // print(removeAdd.Length + " " + removeAddPos.Length);
        
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
        //         //print($"add : {_addTiles[i]} / i : {i}");
        //         removeAddPos[i] = _addTiles[i-_removeTiles.Count];
        //     }
        // }
        //
        // _fogOfWarTilemap.SetTiles(removeAddPos, removeAdd);
        // print($"removeAddPos : {removeAddPos.Length} removeAdd : {removeAdd.Length}");
        
        _removeTiles.Clear();
        _addTiles.Clear();
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