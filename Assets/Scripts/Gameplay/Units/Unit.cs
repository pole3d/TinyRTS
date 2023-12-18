using System.Collections.Generic;
using Common.ActorSystem;
using GameManagement.Players;
using UnityEngine;

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
        public int Damage => UnitData.Damage;
        public int Range => UnitData.Range;
        public float MoveSpeed => UnitData.MoveSpeed;
        public Sprite IconSprite => UnitData.IconSprite;
        public List<UnitData.ActionType> Actions => UnitData.UnitActions;
        public UnitData UnitData { get; set; }
        [field:Space (10)][field:Header("Unit Data")][field:SerializeField] public GameplayData Data { get; set; }

        [SerializeField] private ActorView _view;
        private Vector2? _destination;
        
        private List<Unit> _enemyUnitsInRange = new List<Unit>();


        private void Start()
        {
            Initialize(Data.GetUnitData(UnitData.UnitType));
        }

        public void Initialize(UnitData data)
        {
            UnitData = data;

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
                float moveStep = Time.deltaTime * UnitData.MoveSpeed;
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
                    || unit.UnitData.Team == UnitData.Team)
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
                if (distance > UnitData.Range)
                {
                    continue;
                }
                
                if (closestUnit == null || distance < closestUnitDistance)
                {
                    closestUnit = unit;
                    closestUnitDistance = distance;
                }
            }

            return closestUnit;
        }
        
#if UNITY_EDITOR

        private void OnDrawGizmos()
        {
            Unit unitToAttack = GetUnitToAttack();
            
            Gizmos.color = new Color(1f, 0.92f, 0.02f, 0.5f);
            foreach (Unit unit in _enemyUnitsInRange)
            {
                if (unitToAttack == unit)
                {
                    continue;
                }
                Gizmos.DrawLine(transform.position, unit.transform.position);
            }

            if (UnitData.Team == PlayerTeamEnum.Team1)
            {
                Gizmos.color = Color.red;
                if (unitToAttack != null)
                {
                    Gizmos.DrawLine(transform.position, unitToAttack.transform.position);
                }
            }
            
        }

#endif
    }
}