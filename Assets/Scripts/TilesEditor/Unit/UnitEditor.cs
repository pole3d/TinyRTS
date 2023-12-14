using System;
using Unity.VisualScripting;
using UnityEngine;

namespace Gameplay.Units
{
    [Serializable]
    public class UnitEditor : MonoBehaviour
    {
        [field: SerializeField] public Type UnitType { get; set; }
        [field: SerializeField] public Sprite Sprite { get; set; }
        
        public void SetUnitData(Type type, Sprite sprite)
        {
            UnitType = type;
            Sprite = sprite;
        }
    }
}