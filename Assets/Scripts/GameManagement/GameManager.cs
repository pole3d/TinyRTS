using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameManagement.Players;
using UnityEngine;
using Utilities;

namespace GameManagement
{
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
    
    public class GameManager : Singleton<GameManager>
    {
        //editor fields
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
        private Task _currentTask;
        private Queue<Func<Task>> _taskQueue = new Queue<Func<Task>>();
        
        protected override void InternalAwake()
        {
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

            Board = new BoardManager();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            
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
            UpdateTaskManagement();
            
            Board?.Update();
        }

        #region Task Management

        /// <summary>
        /// Update the task management system by checking if the current task is completed and launching the next one
        /// in the queue. Requires to be put in an Update method.
        /// </summary>
        private void UpdateTaskManagement()
        {
            if (_currentTask == null || _currentTask.IsCompleted == false)
            {
                return;
            }

            _taskQueue.Dequeue();
            _currentTask = null;
            LaunchNextTask();
        }

        /// <summary>
        /// Launch the next task in the task queue if there is one.
        /// </summary>
        private void LaunchNextTask()
        {
            if (_taskQueue.Count <= 0)
            {
                return;
            }

            _currentTask = _taskQueue.Peek()();
        }
        
        /// <summary>
        /// Enqueue a task in the task queue and plays it directly if it's the only one in. 
        /// </summary>
        /// <param name="taskToEnqueue">The Func which contains the task to enqueue. The task can be put as a
        /// func like this : () => Task</param>
        private void EnqueueTask(Func<Task> taskToEnqueue)
        {
            _taskQueue.Enqueue(taskToEnqueue);
            
            if (_taskQueue.Count == 1)
            {
                LaunchNextTask();
            }
        }
        
        /// <summary>
        /// Tell if a task is currently being proceed by the task management system.
        /// </summary>
        /// <returns>A bool whose value tell if a task is being proceed</returns>
        public bool IsDoingTask()
        {
            return _taskQueue.Count > 0;
        }

        #endregion

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
    }
}
