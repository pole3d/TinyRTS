using System;
using Unity.VisualScripting;
using UnityEngine;

namespace Gameplay.Units
{
    [Serializable]
    public class UnitEditorData
    {
        [field: SerializeField] public UnitType UnitType { get; set; }
        [field: SerializeField] public Sprite Sprite { get; set; }
    }
}