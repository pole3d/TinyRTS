using Gameplay.Units;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace TilesEditor
{
    public class UnitButton : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [FormerlySerializedAs("_unitEditorData")] [SerializeField] private UnitEditor unitEditor;

        private void Start()
        {
            _button.onClick.AddListener(() => TilesEditor.Instance.SetCurrentUnit(unitEditor));
        }

        public void SetUnitEditorData(UnitEditor unitEditor)
        {
            this.unitEditor = unitEditor;
        }
    }
}