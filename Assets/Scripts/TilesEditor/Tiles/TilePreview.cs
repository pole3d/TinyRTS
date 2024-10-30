using System;
using System.Collections.Generic;
using UnityEngine;

namespace TilesEditor.Tiles
{
    /// <summary>
    /// A preview of the current selected tile to draw on the tiles editor
    /// </summary>
    public class TilePreview : MonoBehaviour
    {
        public enum PreviewFollowType
        {
            Grid = 0,
            Free = 1,
        }
        
        [field:SerializeField] public SpriteRenderer SpriteRenderer { get; set; }
        [field:SerializeField] public PreviewFollowType FollowType { get; set; }
        [SerializeField] private Vector3 _positionOffset;

        private Dictionary<PreviewFollowType, Func<Vector3, Grid, Vector3>> _followTypes =
            new Dictionary<PreviewFollowType, Func<Vector3, Grid, Vector3>>(); 

        private void Start()
        {
            _followTypes.Add(PreviewFollowType.Grid, (Vector3 mousePos, Grid grid) =>  grid.WorldToCell(new Vector3(mousePos.x, mousePos.y, 0)) + _positionOffset);
            _followTypes.Add(PreviewFollowType.Free, (Vector3 mousePos, Grid grid) =>  mousePos);
        }

        public void SetPosition(Vector3 position, Grid grid)
        {
            SpriteRenderer.transform.position = _followTypes[FollowType].Invoke(position, grid);
        }
    }
}