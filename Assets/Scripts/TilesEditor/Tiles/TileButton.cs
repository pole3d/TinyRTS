using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

namespace TilesEditor.Tiles
{
    /// <summary>
    /// Used to set the tiles buttons data.
    /// </summary>
    public class TileButton : MonoBehaviour
    {
        [SerializeField] private Image _display;
        [SerializeField] private Button _button;

        private TileData _tile;

        private void Start()
        {
            _button.onClick.AddListener(() => TilesEditor.Instance.SetCurrentTile(_tile));
        }

        /// <summary>
        /// Set the tile.
        /// </summary>
        /// <param name="tile"> The tile to set. </param> 
        /// <param name="sprite"> The sprite to display. </param>
        public void SetTileDisplay(TileData tile, Sprite sprite)
        {
            _tile = tile;
            _display.sprite = sprite;
        }
    }
}