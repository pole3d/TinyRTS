using System;
using UnityEngine;
using UnityEngine.UI;

namespace TilesEditor.Tiles
{
    /// <summary>
    /// Set the tilemap buttons values.
    /// </summary>
    public class TilemapButton : MonoBehaviour
    {
        public TilemapData TilemapData { get; private set; }

        private void Start()
        {
            GetComponent<Button>().onClick.AddListener(DisplayTiles);
        }
        
        /// <summary>
        /// Display the button with the corresponding tile.
        /// </summary>
        private void DisplayTiles()
        {
            foreach (TileButton button in TilemapData.TilesButtonsAssociated)
            {
                button.gameObject.SetActive(true);
                TilesEditor.Instance.DontDisplayTiles(this);
            }
        }

        public void SetTilemapData(TilemapData tilemapData)
        {
            TilemapData = tilemapData;
        }
    }
}