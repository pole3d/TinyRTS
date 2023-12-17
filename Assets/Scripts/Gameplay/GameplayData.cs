
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameplayData", menuName = "GameplayData")]
public class GameplayData : ScriptableObject
{
    [field:SerializeField]public List<UnitData> Units { get; set; }

    public UnitData GetUnitData(UnitData.Type type)
    {
        foreach (var unit in Units)
        {
            if (unit.UnitType == type)
            {
                return unit;
            }
        }
        return null;
    }
}