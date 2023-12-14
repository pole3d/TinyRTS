using System;
using System.Numerics;

namespace TilesEditor.Units
{
    [Serializable]
    public class UnitForEditorData
    {
        public UnitData.Type UnitType { get; set; }
        public Vector2 Position { get; set; }
    }
}