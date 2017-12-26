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

        public TaskManager(int threadCount)
        {
            if (threadCount < 1) threadCount = 1;
            m_ThreadCount = threadCount;
        }

        public void RegisterTask(ITask task)
        {
            if (null == task) throw new ArgumentNullException(nameof(task));

            lock (m_Locker)
            {
                if (!m_TaskList.ContainsKey(task.TaskTypeName))
                    m_TaskList.Add(task.TaskTypeName, task);
            }
        }

        public void Start()
        {
            for (int i = 0; i < m_ThreadCount; i++)
            {
                Thread t = new Thread(this.StartOneThread);
                t.IsBackground = true;
                t.Start();
            }
            Thread t2 = new Thread(this.RefreshAllTaskUIDList);
            t2.IsBackground = true;
            t2.Start();
        }
        private void StartOneThread()
        {
            while (true)
            {
                ITask task = this.GetRandomTask();
                if (null != task) task.DoNextTask();
                Thread.Sleep(100);
            }
        }
        private void RefreshAllTaskUIDList()
        {
            while (true)
            {
                lock (m_Locker)
                {
                    foreach (var item in m_TaskList)
                    {
                        ThreadPool.QueueUserWorkItem(x => { item.Value.RefreshTaskUIDList(); });
                    }
                }
                Thread.Sleep(10 * 1000);
            }
        }

        private Random m_Random = new Random();
        private ITask GetRandomTask()
        {
            ITask task = null;
            lock (m_Locker)
            {
                IList<string> list = new List<string>(m_TaskList.Keys);
                string randomKey = list[m_Random.Next(list.Count)];
                task = m_TaskList[randomKey];
            }
            return task;
        }
    }
}