using System;
using UnityEngine;

namespace Common.ActorSystem
{
    /// <summary>
    /// The ActorViewAction class defines actions associated with the entity.
    /// Actions are time-limited animations that return to the state animation when they complete.
    /// These actions are primarily used with non-looping animations.
    /// Action name must match with an animation
    /// </summary>
    [Serializable]
    public class ActorViewAction
    {
        /// <summary>
        /// The name must match the animation's name
        /// </summary>
        [field: SerializeField] public string Name { get; private set; }
        /// <summary>
        /// Specifies the action that will be played once the current action is completed.
        /// </summary>
        [field: SerializeField] public string NextAction { get; private set; }

    }

}