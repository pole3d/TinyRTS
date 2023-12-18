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

            CreateTestUnitAt(Vector2.one * 7);
            CreateTestUnitAt(Vector2.one * 8);
            CreateTestUnitAt(Vector2.one * 9);
            CreateTestUnitAt(Vector2.one * 10);
            CreateTestUnitAt(Vector2.one * 11);
            CreateTestUnitAt(Vector2.one * 12);
            CreateTestUnitAt(Vector2.one * 13);
            CreateTestUnitAt(Vector2.one * 14);
            CreateTestUnitAt(Vector2.one * 15);
            CreateTestUnitAt(Vector2.one * 16);
            CreateTestUnitAt(Vector2.one * 17);
            CreateTestUnitAt(Vector2.one * 18);
            CreateTestUnitAt(Vector2.one * 19);
            CreateTestUnitAt(Vector2.one * 20);
            CreateTestUnitAt(Vector2.one * 21);
            CreateTestUnitAt(Vector2.one * 22);
            CreateTestUnitAt(Vector2.one * 23);
            CreateTestUnitAt(Vector2.one * 24);
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
