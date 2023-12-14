using System;
using System.Collections.Generic;
using Common.ActorSystem;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// Represents a unit in the rts,
/// the unit can respond to order, like moving
/// a unit can be hit and die
/// </summary>
public class Unit : MonoBehaviour
{
    public int Life { get; private set; }
    public int Damage => _unitData.Damage;
    public int Range => _unitData.Range;
    public float MoveSpeed => _unitData.MoveSpeed;
    public Sprite IconSprite => _unitData.IconSprite;
    public List<UnitData.ActionType> Actions => _unitData.UnitActions;

    [SerializeField] ActorView _view;
    Vector2? _destination;

    [Space (10)][Header("Unit Data")]
    [SerializeField] GameplayData _data;
    public UnitData _unitData;

    private void Start()
    {
        Initialize(_data.GetUnitData(_unitData.UnitType));
    }

    public void Initialize(UnitData data)
    {
        _unitData = data;

        Life = data.Life;
    }

    public void Move(Vector2 destination)
    {
        _destination = destination;
        _view.PlayState("walk");
    }

    public void Update()
    {
        Move();
    }

    void Move()
    {
        if (_destination != null)
        {
            Vector3 direction = _destination.Value - new Vector2(transform.position.x, transform.position.y);
            float distance = direction.magnitude;
            float moveStep = Time.deltaTime * _unitData.MoveSpeed;
            direction.Normalize();

            if (moveStep >= distance)
            {
                transform.position = _destination.Value;
                _destination = null;
                _view.PlayState("idle");
            }
            else
            {
                transform.position = transform.position + moveStep * direction;
            }
        }
    }
}