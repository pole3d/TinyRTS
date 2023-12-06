using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameManagement.Players;
using Gameplay.Units;
using Pathfinding;
using TilesEditor;
using UnityEngine;
using Utilities;

namespace GameManagement
{
    /// <summary>
    /// This class contains the game's data 
    /// </summary>
    public class GameManagerGameData
    {
        public GameManagerGameData(LocalPlayerManager localPlayer, AIPlayerManager aiPlayer)
        {
            LocalPlayer = localPlayer;
            AIPlayer = aiPlayer;
        }

        public LocalPlayerManager LocalPlayer { get; internal set; }
        public AIPlayerManager AIPlayer { get; internal set; }
    }
    
    
    /// <summary>
    /// This class manage the game's instances and reference important values relative to the game
    /// </summary>
    public class GameManager : Singleton<GameManager>
    {
        [field:Header("References")] [field:SerializeField] public MapLoader MapLoader { get; private set; }
        [field:SerializeField] public FogOfWar FogOfWar { get; private set; }
        [field:SerializeField] public Unit UnitPrefab { get; private set; }
        
        [Space(5), Header("Teams")]
        [SerializeField] private PlayerTeamEnum _playerTeam;
        [SerializeField] private PlayerTeamEnum _enemyTeam;

        //references
        public BoardManager Board { get; private set; }
        
        //datas
        public GameManagerGameData GameData { get; private set; }
        
        //events
        public Action OnGameStarted { get; set; }
        public Action OnGameWon { get; set; }
        public Action OnGameLost { get; set; }
        
        //task management
        public TaskManager TaskManager { get;set; }
        
        protected override void InternalAwake()
        {
            TaskManager = new TaskManager();

            if (_playerTeam == _enemyTeam)
            {
                throw new Exception("Player's and Enemy's team are the same");
            }
            GameData = new GameManagerGameData(
                new LocalPlayerManager(_playerTeam, 0), 
                new AIPlayerManager(_enemyTeam, 1));
            
            OnGameStarted += GameStart;
            OnGameWon += GameWon;
            OnGameLost += GameLost;

            Board = new BoardManager(this);
            
            MapLoader.Initialize();
            FogOfWar.Initialize();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            
            OnGameStarted -= GameStart;
            OnGameWon -= GameWon;
            OnGameLost -= GameLost;
        }

        private void Start()
        {
            OnGameStarted.Invoke();
        }

        private void Update()
        {
            TaskManager.UpdateTaskManagement();
            
            Board?.Update();
        }

        #region Game States

        private void GameStart()
        {
            
        }

        private void GameLost()
        {
            
        }

        private void GameWon()
        {
            
        }

        #endregion
        
        #if UNITY_EDITOR

        private Dictionary<Vector2Int, Color> _dicoNodeGizmos = new Dictionary<Vector2Int, Color>();

        public void DrawGizmoAt(Vector2Int positon, Color color)
        {
            if (_dicoNodeGizmos.ContainsKey(positon))
            {
                _dicoNodeGizmos[positon] = color;
                return;
            }
            
            _dicoNodeGizmos.Add(positon,color);
        }
        
        public void OnDrawGizmos()
        {
            foreach (var node in _dicoNodeGizmos)
            {
                Gizmos.color = node.Value;
                PathFinding pathFinding = PathFinding.Instance;
                Vector3 worldPosition = pathFinding.Grid.GetWorldPosition(node.Key.x, node.Key.y) + (Vector3.one * pathFinding.Grid.CellSize * 0.5f);
                Gizmos.DrawSphere(worldPosition, 1f);
            }
        }

#endif
    }
}
