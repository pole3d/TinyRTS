using System;
using System.Collections.Generic;
using Common.ActorSystem;
using UnityEngine;
using UnityEngine.Serialization;

namespace Gameplay.Units
{
    /// <summary>
    /// Represents a unit in the rts,
    /// the unit can respond to order, like moving
    /// a unit can be hit and die
    /// </summary>
    public class Unit : MonoBehaviour
    {
        public int Life { get; private set; }
        public int Damage => Data.Damage;
        public int Range => Data.Range;
        
        public UnitData Data { get; set; }
        
        [SerializeField] ActorView _view;

        private Vector2? _destination;
        private List<Unit> _enemyUnitsInRange = new List<Unit>();
        
        public void Initialize(UnitData data)
        {
            Data = data;

            Life = data.Life;
        }

        public void Move(Vector2 destination)
        {
            _destination = destination;
            _view.PlayState("walk");
        }

        public void Update()
        {
            CheckForOtherUnitsInRange();
            Move();
        }

        private void Move()
        {
            if (_destination != null)
            {
                Vector3 direction = _destination.Value - new Vector2(transform.position.x,transform.position.y);
                float distance = direction.magnitude;
                float moveStep = Time.deltaTime * Data.MoveSpeed;
                direction.Normalize();

                if ( moveStep >= distance)
                {
                    transform.position = _destination.Value;
                    _destination = null;
                    _view.PlayState("idle");
                }
                else
                {
                    transform.position = transform.position +  moveStep * direction;
                }
            }
        }

        private void CheckForOtherUnitsInRange()
        {
            RaycastHit2D[] hits = new RaycastHit2D[100];
            _enemyUnitsInRange.Clear();
            Physics2D.CircleCastNonAlloc(transform.position, 5f, Vector2.zero, hits);
            foreach (RaycastHit2D hit in hits)
            {
                if (hit.collider == null
                    || hit.collider.TryGetComponent(out Unit unit) == false
                    || unit.Data.Team == Data.Team)
                {
                    continue;
                }

                _enemyUnitsInRange.Add(unit);
            }
        }

        private Unit GetUnitToAttack()
        {
            Unit closestUnit = null;
            float closestUnitDistance = float.MaxValue;
            foreach (Unit unit in _enemyUnitsInRange)
            {
                float distance = Vector2.Distance(transform.position, unit.transform.position);
                if (distance < Data.Range)
                {
                    if (closestUnit == null || distance < closestUnitDistance)
                    {
                        closestUnit = unit;
                        closestUnitDistance = distance;
                    }
                }
            }

            return closestUnit;
        }
        
#if UNITY_EDITOR

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            foreach (Unit unit in _enemyUnitsInRange)
            {
                Gizmos.DrawLine(transform.position, unit.transform.position);
            }
            
            Gizmos.color = Color.red;
            Unit unitToAttack = GetUnitToAttack();
            if (unitToAttack != null)
            {
                Gizmos.DrawLine(transform.position, unitToAttack.transform.position);
            }
        }

#endif
    }
}
