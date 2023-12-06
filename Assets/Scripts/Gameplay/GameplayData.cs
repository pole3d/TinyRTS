
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameplayData", menuName = "GameplayData")]
public class GameplayData : ScriptableObject
{
    [field:SerializeField]public List<UnitData> Units { get; set; }

}