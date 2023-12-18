﻿using UnityEngine;

namespace Building
{
    public class AnimatedBuilding : Building
    {
        public float TimeBetweenFrames;
        public Sprite[] Frames;

        private int _currentFrameIndex;
        private float _currentFrameTime;

        protected override void Update()
        {
            base.Update();

            UpdateFrameTimer();
        }

        private void UpdateFrameTimer()
        {
            _currentFrameTime += Time.deltaTime;
            if (_currentFrameTime < TimeBetweenFrames)
            {
                return;
            }

            _currentFrameTime -= TimeBetweenFrames;
            NextFrame();
        }

        private void NextFrame()
        {
            _currentFrameIndex++;
            _currentFrameIndex %= Frames.Length;
            
            SpriteRenderer.sprite = Frames[_currentFrameIndex];
        }
    }    
}