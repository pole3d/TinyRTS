using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GameManagement
{
    /// <summary>
    /// This class is managing asynchronous tasks and ca be used to enqueue tasks that plays one after another 
    /// </summary>
    public class TaskManager
    {
        private Task _currentTask;
        private Queue<Func<Task>> _taskQueue = new Queue<Func<Task>>();
        
        /// <summary>
        /// Update the task management system by checking if the current task is completed and launching the next one
        /// in the queue. Requires to be put in an Update method.
        /// </summary>
        public void UpdateTaskManagement()
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
    }
}