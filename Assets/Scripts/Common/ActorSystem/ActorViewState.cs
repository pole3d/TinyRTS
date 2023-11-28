using System;
using UnityEngine;

namespace Common.ActorSystem
{
    /// <summary>
    /// The ActorViewState class defines states representing the entity, such as idle, walk, jump...
    /// These states are primarily used with looping animations.
    /// State name must match with an animation
    /// </summary>
    [Serializable]
    public class ActorViewState
    {
        /// <summary>
        /// The name must match the animation's name
        /// </summary>
        [field: SerializeField] public string Name { get; private set; }

        /// <summary>
        /// The ID property is used to efficiently retrieve the state by a unique identifier rather than relying on the name.
        /// </summary>
        public int ID { get; set; }
    }

}