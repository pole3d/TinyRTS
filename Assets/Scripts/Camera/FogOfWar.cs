using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

public class FogOfWar : MonoBehaviour
{
    [SerializeField] private Tilemap _fogOfWarTilemap;
    [SerializeField] private Transform _viewer;
    [SerializeField] private TileBase _discoveredTile;
    [Range(1, 15)] [SerializeField] private int _fogRadius = 8;

    private Vector3 _lastPlayerPosition;

    private readonly HashSet<Vector3Int> _discoveredTilesList = new HashSet<Vector3Int>();
    
    private const float MinDistancePlayerMove = 0.01f;


    private void Start()
    {
        _lastPlayerPosition = _viewer.position;
        
        CheckDiscoveredTiles();
    }

    private void Update()
    {
        CheckIfPlayerMove();
    }

    private void CheckIfPlayerMove()
    {
        if (Vector3.Distance(_lastPlayerPosition, _viewer.position) > MinDistancePlayerMove)
        {
            CheckDiscoveredTiles();
            
            _lastPlayerPosition = _viewer.position;
        }
    }

    private void CheckDiscoveredTiles()
    {
        Vector3Int playerTilePos = _fogOfWarTilemap.WorldToCell(_viewer.position);

        List<Vector3Int> tilesToRemove = new List<Vector3Int>();

        // Check for discovered tiles if they are outside the _fogRadius range
        foreach (Vector3Int discoveredTile in _discoveredTilesList)
        {
            if (Vector3Int.Distance(playerTilePos, discoveredTile) > _fogRadius)
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
        
        AddDiscoveredTiles(playerTilePos);
    }

    private void AddDiscoveredTiles(Vector3Int playerTilePos)
    {
        // For all tiles in the radius
        for (int x = playerTilePos.x - _fogRadius; x <= playerTilePos.x + _fogRadius; x++)
        {
            for (int y = playerTilePos.y - _fogRadius; y <= playerTilePos.y + _fogRadius; y++)
            {
                Vector3Int cellPosition = new Vector3Int(x, y, 0);

                // Check if it is inside the _fogRadius
                if (Vector3Int.Distance(playerTilePos, cellPosition) <= _fogRadius)
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
        Gizmos.DrawWireSphere(_viewer.position, _fogRadius);
    }
}