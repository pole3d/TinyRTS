using System.Collections.Generic;
using Common.ActorSystem;
using UnityEngine;

namespace Buildings
{
    public class Building : MonoBehaviour
    {
        [Header("References")]
        public SpriteRenderer SpriteRenderer;
        public ActorView View;
        
        [Header("Building")]
        public string Name;
        public Sprite InitialSprite;
        
        [Header("Stages")]
        private readonly Queue<BuildStage> _buildStages = new Queue<BuildStage>();

        private BuildStage _currentStage;
        private float _targetBuildTime;
        private float _currentStageTimer;

        public bool IsGhost { get; set; }
        public bool IsBuilt { get; private set; }

        public GameplayData GameplayData;
        private UnitData _data;

        private void Awake()
        {
            _data = GameplayData.Units.Find(u => u.UnitType == UnitData.Type.GoldMine);
            
            foreach (BuildStage stage in _data.BuildStages)
            {
                _buildStages.Enqueue(stage);
            }
        }

        private void Start()
        {
            SpriteRenderer.sprite = InitialSprite;
        }

        protected virtual void Update()
        {
            UpdateBuildStatus();
            View.Update();
        }

        private void UpdateBuildStatus()
        {
            if (IsGhost || IsBuilt)
            {
                return;
            }

            if (_currentStage == null)
            {
                if (_buildStages.TryDequeue(out _currentStage))
                {
                    _targetBuildTime = _data.BuildTime * (_currentStage.PercentageOfBuildTime / 100f);
                    View.PlayState(_currentStage.Name);
                    return;
                }
                    
                PlayBuiltAnimation();
                IsBuilt = true;

                return;
            }

            _currentStageTimer += Time.deltaTime;
            if (_currentStageTimer < _targetBuildTime)
            {
                return;
            }
            
            _currentStageTimer = 0;
            _currentStage = null;
        }

        public void PlayBuiltAnimation()
        {
            View.PlayState(_data.BuiltStageName);
        }
    }    
}
