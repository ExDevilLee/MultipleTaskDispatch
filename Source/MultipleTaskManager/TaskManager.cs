using System;
using System.Collections.Generic;
using System.Threading;

namespace MultipleTaskManager
{
    public class TaskManager
    {
        private object m_Locker = new object();
        private int m_ThreadCount;
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

        private int m_IntervalOfTaskMilliseconds = 100;
        public int IntervalOfTaskMilliseconds
        {
            get { return m_IntervalOfTaskMilliseconds; }
            set
            {
                if (value <= 0) throw new ArgumentOutOfRangeException(paramName: nameof(value),
                    message: $"{nameof(IntervalOfTaskMilliseconds)}'s value must be greater than zero.");
                m_IntervalOfTaskMilliseconds = value;
            }
        }

        public TaskManager(int threadCount)
        {
            if (threadCount < 1) threadCount = 1;
            m_ThreadCount = threadCount;
        }
        public TaskManager(int threadCount, params ITask[] tasks)
            : this(threadCount)
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
            for (int i = 0; i < m_ThreadCount; i++)
            {
                new Thread(this.StartOneThread) { IsBackground = true }.Start();
            }
            new Thread(this.RefreshAllTaskUIDList) { IsBackground = true }.Start();
        }
        private void StartOneThread()
        {
            while (true)
            {
                this.GetRandomTask()?.DoNextTask();
                Thread.Sleep(this.IntervalOfTaskMilliseconds);
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

        private static Random m_Random = new Random(new object().GetHashCode());
        private ITask GetRandomTask()
        {
            ITask task = null;
            lock (m_Locker)
            {
                IList<string> list = new List<string>(m_TaskList.Keys);
                if (list.Count > 0)
                {
                    string randomKey = list[m_Random.Next(list.Count)];
                    task = m_TaskList[randomKey];
                }
            }
            return task;
        }
    }
}