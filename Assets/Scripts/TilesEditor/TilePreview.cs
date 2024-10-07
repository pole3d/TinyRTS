using UnityEngine;

namespace TilesEditor
{
    /// <summary>
    /// A preview of the current selected tile to draw on the tiles editor
    /// </summary>
    public class TilePreview : MonoBehaviour
    {
        [field:SerializeField] public SpriteRenderer SpriteRenderer { get; set; }
        [SerializeField] private Vector3 _positionOffset;

        public void SetPosition(Vector3 position)
        {
            SpriteRenderer.transform.position = position + _positionOffset;
        }
    }
}