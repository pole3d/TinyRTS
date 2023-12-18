using System;
using UnityEngine;

namespace TilesEditor.Units
{
    /// <summary>
    /// Class with the values needed to associate a unit for editor data to the unit data corresponding during the loading step, outside the editor scene. 
    /// </summary>
    [Serializable]
    public class UnitForEditorData
    {
        [field:SerializeField] public UnitData.Type UnitType { get; set; }
        [field:SerializeField] public Vector2 Position { get; set; }
    }
}