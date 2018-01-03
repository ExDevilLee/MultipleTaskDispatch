using System;
using System.Collections.Generic;
using System.Threading;

namespace MultipleTaskManager
{
    public class TaskManagerByThreadPool
    {
        private object m_Locker = new object();
        private IDictionary<string, ITask> m_TaskList = new Dictionary<string, ITask>();

        private int m_IntervalOfAutoRefreshSeconds = 10;
        public int IntervalOfAutoRefreshSeconds
        {
            get { return m_IntervalOfAutoRefreshSeconds; }
            set
            {
                if (value <= 0) throw new ArgumentOutOfRangeException(paramName: nameof(value),
                    message: $"{nameof(IntervalOfAutoRefreshSeconds)}'s value must be greater than zero.");
                m_IntervalOfAutoRefreshSeconds = value;
            }
        }

        public TaskManagerByThreadPool(params ITask[] tasks)
        {
            this.RegisterTask(tasks);
        }

        public void RegisterTask(params ITask[] tasks)
        {
            if (null == tasks || tasks.Length == 0) return;

            lock (m_Locker)
            {
                foreach (var task in tasks)
                {
                    if (null == task) continue;
                    if (!m_TaskList.ContainsKey(task.TaskTypeName))
                        m_TaskList.Add(task.TaskTypeName, task);
                }
            }
        }

        public void Start()
        {
            lock (m_Locker)
            {
                foreach (var task in m_TaskList)
                {
                    task.Value.TaskAdded -= TaskAdded;
                    task.Value.TaskAdded += TaskAdded;
                    this.AddTaskToThreadPool(task.Value);
                }
            }
            new Thread(this.RefreshAllTaskUIDList) { IsBackground = true }.Start();
        }
        private void TaskAdded(object sender, EventArgs e)
        {
            this.AddTaskToThreadPool(sender as ITask);
        }
        private void AddTaskToThreadPool(ITask task)
        {
            if (null == task) return;
            for (int i = 0; i < task.RemainTaskCount; i++)
            {
                ThreadPool.QueueUserWorkItem(x => { task.DoNextTask(); });
            }
        }

        private void RefreshAllTaskUIDList()
        {
            while (true)
            {
                Thread.Sleep(this.IntervalOfAutoRefreshSeconds * 1000);

                lock (m_Locker)
                {
                    foreach (var item in m_TaskList)
                    {
                        ThreadPool.QueueUserWorkItem(x => { item.Value.RefreshTaskUIDList(); });
                    }
                }
            }
        }
    }
}