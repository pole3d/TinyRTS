using UnityEngine;

namespace TilesEditor.Units
{
    /// <summary>
    /// Unit class but for the editor scene only. Contains the values needed to save and load the units.
    /// </summary>
    public class UnitForEditor : MonoBehaviour
    {
        public UnitForEditorData UnitForEditorData { get; set; }
        [field:SerializeField] public SpriteRenderer SpriteRenderer { get; set; }

        public void SetDisplay(Sprite sprite)
        {
            SpriteRenderer.sprite = sprite;
        }
    }
}