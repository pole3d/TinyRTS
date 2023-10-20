using System;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

namespace TilesEditor
{
    public class TileButton : MonoBehaviour
    {
        [SerializeField] private Tile _tile;
        [SerializeField] private Image _display;

        /// <summary>
        /// Set the tile.
        /// </summary>
        /// <param name="tile"></param>
        /// <param name="sprite"></param>
        public void SetTile(Tile tile, Sprite sprite)
        {
            _tile = tile;
            _display.sprite = sprite;
        }
    }
}