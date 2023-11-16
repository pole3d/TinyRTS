using UnityEngine;

namespace Building
{
    public class Building : MonoBehaviour
    {
        public SpriteRenderer SpriteRenderer;
        
        public string Name;
        public Sprite Sprite;

        public float BuildTime;
        private float _currentBuildTime;
        
        public bool IsGhost { get; set; }

        private void Start()
        {
            SpriteRenderer.sprite = Sprite;
        }

        protected virtual void Update()
        {
            UpdateBuildStatus();
        }

        private void UpdateBuildStatus()
        {
            if (IsGhost)
            {
                return;
            }
            
            if (!IsBeingBuilt())
            {
                return;
            }
            
            // todo: show the building is being built
            
            _currentBuildTime += Time.deltaTime;
            if (!IsBeingBuilt())
            {
                // todo: show the building has finished being built
            }
        }

        public bool IsBeingBuilt()
        {
            return _currentBuildTime < BuildTime;
        }
    }    
}
