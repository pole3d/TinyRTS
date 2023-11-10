using UnityEngine;

namespace Building {
    
    public class Building : MonoBehaviour {

        public SpriteRenderer SpriteRenderer;
        
        public string Name;
        public Sprite Sprite;

        private void Start() {
            SpriteRenderer.sprite = Sprite;
        }
    }    
}
