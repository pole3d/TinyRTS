using UnityEngine;

namespace TilesEditor.Units
{
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