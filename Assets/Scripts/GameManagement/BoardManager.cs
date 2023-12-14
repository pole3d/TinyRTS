using GameManagement.Players;
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
            CreateTestUnitAt(Vector2.zero, PlayerTeamEnum.Team1);
            CreateTestUnitAt(Vector2.one, PlayerTeamEnum.Team1);
            CreateTestUnitAt(Vector2.one * 5, PlayerTeamEnum.Team2);
            CreateTestUnitAt(Vector2.one * 4, PlayerTeamEnum.Team2);
        }

        public void Update()
        {
            
        }

        private void CreateTestUnitAt(Vector2 position, PlayerTeamEnum team)
        {
            Unit unit = Object.Instantiate(_gameManager.UnitPrefab, position, Quaternion.identity);
            unit.Data = new UnitData();
            unit.Data.Team = team;
            _gameManager.FogOfWar.AddNewViewer(unit.transform, 5);
        }
    }
}
