using System;
using UnityEngine;

namespace TilesEditor.Units
{
    [Serializable]
    public class UnitForEditorData
    {
        [field:SerializeField] public UnitData.Type UnitType { get; set; }
        [field:SerializeField] public Vector2 Position { get; set; }
    }
}