using Common.ActorSystem;
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
        public int Damage => _data.Damage;
        public int Range => _data.Range;

        [SerializeField] ActorView _view;

        UnitData _data;
        Vector2? _destination;

        public void Initialize(UnitData data)
        {
            _data = data;

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
                Vector3 direction = _destination.Value - new Vector2(transform.position.x,transform.position.y);
                float distance = direction.magnitude;
                float moveStep = Time.deltaTime * _data.MoveSpeed;
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

    }
}
