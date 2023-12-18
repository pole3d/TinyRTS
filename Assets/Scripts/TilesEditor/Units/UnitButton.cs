using UnityEngine;
using UnityEngine.UI;

namespace TilesEditor.Units
{
    /// <summary>
    /// Set the unit buttons values.
    /// </summary>
    public class UnitButton : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private Image _image;
        private UnitForEditorData _data;

        private void Start()
        {
            _button.onClick.AddListener(() => TilesEditor.Instance.SetCurrentUnit(_data));
        }

        public void SetUnitData(UnitForEditorData data, Sprite sprite)
        {
            _data = data;
            _image.sprite = sprite;
        }
    }
}