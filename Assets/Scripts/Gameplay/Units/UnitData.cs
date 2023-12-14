using System;
using GameManagement.Players;
using UnityEngine;
/// <summary>
/// Contains data about units like initial life, damage, range etc... 
/// </summary>
[Serializable]
public class UnitData
{
    [field: SerializeField] public PlayerTeamEnum Team { get; set; }
    [field: SerializeField] public UnitType UnitType{ get; set; }
    [field: SerializeField] public int Life { get; set; }
    [field: SerializeField] public int Damage { get; set; }
    [field: SerializeField] public int Range { get; set; }
    [field: SerializeField] public float MoveSpeed { get; set; }
}
    public enum UnitType
    {
        Builder,
        Warrior
    }


