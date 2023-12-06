using Gameplay.Units;
using UnityEngine;

namespace GameManagement
{
    /// <summary>
    /// This class manage everything relative to the board of the game and reference infos about it
    /// </summary>
    public class BoardManager
    {
        private GameManager _gameManager;
        
        public BoardManager(GameManager gameManager)
        {
            _gameManager = gameManager;
            CreateTestUnitAt(Vector2.zero);
        }

        public void Update()
        {
            
        }

        private void CreateTestUnitAt(Vector2 position)
        {
            Unit unit = Object.Instantiate(_gameManager.UnitPrefab, position, Quaternion.identity);
            _gameManager.FogOfWar.AddNewViewer(unit.transform, 5);
        }
    }
}
