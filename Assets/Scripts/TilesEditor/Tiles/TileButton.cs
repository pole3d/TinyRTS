using UnityEngine;
using UnityEngine.UI;

namespace TilesEditor.Tiles
{
    /// <summary>
    /// Set the tile buttons values.
    /// </summary>
    public class TileButton : MonoBehaviour
    {
        [SerializeField] private Image _display;
        [SerializeField] private Button _button;

        private TileData _tile;

        public void SetTileDisplay(TileData tile, Sprite sprite)
        {
            _tile = tile;
            _display.sprite = sprite;
            _button.onClick.AddListener(() => TilesEditor.Instance.SetCurrentTile(_tile));
        }
    }
}