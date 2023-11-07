using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

namespace TilesEditor.Tiles
{
    public class TileButton : MonoBehaviour
    {
        [SerializeField] private TileData _tile;
        [SerializeField] private Image _display;

        private Button _button;

        private void Start()
        {
            _button = GetComponent<Button>();
            _button.onClick.AddListener(SetCurrentTile);
        }

        /// <summary>
        /// Update the current tile on click.
        /// </summary>
        private void SetCurrentTile()
        {
            MainEditor.Instance.CurrentTile = _tile;
            MainEditor.Instance.UpdateCurrentTile();
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