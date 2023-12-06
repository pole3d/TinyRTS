using Gameplay.Units;
using UnityEngine;
using UnityEngine.UI;

namespace TilesEditor
{
    public class UnitButton : MonoBehaviour
    {
        [SerializeField] private Button _button;

        private UnitEditorData _unitEditorData;

        private void Start()
        {
            _button.onClick.AddListener(() => TilesEditor.Instance.SetCurrentUnit(_unitEditorData));
        }

        public void SetUnitEditorData(UnitEditorData unitEditorData)
        {
            _unitEditorData = unitEditorData;
        }
    }
}