using System;
using System.Collections.Generic;
using UnityEngine;

namespace Common.Graphics
{
    /// <summary>
    /// A container asset for storing animations associated with an entity, each accessible by its unique name.
    /// This asset allows you to store and retrieve animations based on their names.
    /// TODO: Consider optimizing retrieval with an index.
    /// </summary>
    [CreateAssetMenu]
    public class SpritesAnimationDatas : ScriptableObject
    {
        /// <summary>
        /// A class used for storing data related to a single animation,
        /// including its name, playback speed (FPS), and associated sprites.
        /// </summary>
        [Serializable]
        public class AnimationDatas
        {
            [field: SerializeField] public string Name { get; private set; }
            [field: SerializeField] public int FPS { get; private set; }
            [field: SerializeField] public List<Sprite> Sprites { get; private set; }
        }

        public List<AnimationDatas> Animations;

        Dictionary<string, AnimationDatas> _animations;

        /// <summary>
        /// Populates a dictionary with animation data for quick access
        /// </summary>
        private void Initialize()
        {
            _animations = new Dictionary<string, AnimationDatas>();

            foreach (var item in Animations)
            {
                _animations.Add(item.Name, item);
            }
        } 

        /// <summary>
        /// Retrieves an animation with the given name
        /// returns null if no animation is found
        /// </summary>
        /// <param name="animationName"></param>
        /// <returns></returns>
        public AnimationDatas GetAnimation(string animationName)
        {
            if (_animations == null)
                Initialize();

            AnimationDatas animation;

            if (_animations.TryGetValue(animationName, out animation) == false)
            {
                throw new Exception($"can't find animation {animationName} in {this.name}");
            }

            return animation;
        } 

    } 

}