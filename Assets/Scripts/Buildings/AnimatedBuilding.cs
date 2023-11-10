using UnityEngine;

namespace Building {
    
    public class AnimatedBuilding : Building {

        public float TimeBetweenFrames;
        public Sprite[] Frames;

        private int _currentFrameIndex;
        private float _currentFrameTime;

        private void Update() {
            _currentFrameTime += Time.deltaTime;
            if (_currentFrameTime > TimeBetweenFrames) {
                _currentFrameTime -= TimeBetweenFrames;

                NextFrame();
            }
        }

        private void NextFrame() {
            _currentFrameIndex++;
            _currentFrameIndex %= Frames.Length;
            
            SpriteRenderer.sprite = Frames[_currentFrameIndex];
        }
    }    
}