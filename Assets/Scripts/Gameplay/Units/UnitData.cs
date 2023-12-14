using System;
using System.Collections.Generic;
using Buildings;
using UnityEngine;
/// <summary>
/// Contains data about units like initial life, damage, range etc... 
/// </summary>
[Serializable]
public class UnitData
{
    public enum Type
    {
        Builder,
        Warrior,
        GoldMine,
    }

    [field: SerializeField] public Type UnitType{ get; set; }
    [field: SerializeField] public int Life { get; set; }
    [field: SerializeField] public int Damage { get; set; }
    [field: SerializeField] public int Range { get; set; }
    [field: SerializeField] public float MoveSpeed { get; set; }

    [field: SerializeField] public string BuiltStageName { get; set; }
    [field: SerializeField] public float BuildTime { get; set; }
    [field: SerializeField] public List<BuildStage> BuildStages { get; set; }
}


