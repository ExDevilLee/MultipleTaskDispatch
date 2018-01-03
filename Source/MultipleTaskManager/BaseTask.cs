using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Linq;

namespace MultipleTaskManager
{
    public abstract class BaseTask<TKey> : ITask
    {
        private readonly object m_Locker = new object();
        private IDictionary<TKey, bool> m_TaskUIDList = new Dictionary<TKey, bool>();
        private static bool TKeyIsValueType => typeof(TKey).IsValueType;

        public event EventHandler TaskAdded;
        protected virtual void OnTaskAdded()
        {
            this.TaskAdded?.Invoke(this, new EventArgs());
        }

        public abstract string TaskTypeName { get; }
        public int MaxTaskCount { get; }
        public int RemainTaskCount
        {
            get
            {
                lock (m_Locker)
                {
                    return m_TaskUIDList.Where(x => !x.Value).ToArray().Length;
                }
            }
        }

        private Stopwatch m_StopwatchForRefresh = new Stopwatch();
        protected BaseTask(int maxTaskCount)
        {
            this.MaxTaskCount = maxTaskCount;
            m_StopwatchForRefresh.Start();
            ThreadPool.QueueUserWorkItem(x => { this.RefreshTaskUIDList(true); });
        }

        protected abstract IList<TKey> GetTaskUIDList(int top);

        protected virtual uint MinRefreshMilliseconds => 1000;
        public void RefreshTaskUIDList()
        {
            this.RefreshTaskUIDList(false);
        }
        private void RefreshTaskUIDList(bool init)
        {
            lock (m_Locker)
            {
                if (!init && m_StopwatchForRefresh.Elapsed.TotalMilliseconds <= this.MinRefreshMilliseconds) return;

                IList<TKey> taskList = this.GetTaskUIDList(this.MaxTaskCount);
                if (null == taskList || taskList.Count == 0)
                {
                    m_StopwatchForRefresh.Restart();
                    return;
                }

                bool hasNewTask = false;
                foreach (var uid in taskList)
                {
                    if (!TKeyIsValueType && null == uid) continue;
                    if (m_TaskUIDList.ContainsKey(uid)) continue;
                    m_TaskUIDList.Add(uid, false);
                    hasNewTask = true;
                    if (m_TaskUIDList.Count >= this.MaxTaskCount) break;
                }
                if (hasNewTask) this.OnTaskAdded();
                m_StopwatchForRefresh.Restart();
            }
        }

        protected virtual bool TryGetNextTaskUID(out TKey taskUID)
        {
            taskUID = default(TKey);
            lock (m_Locker)
            {
                if (m_TaskUIDList.Count == 0) return false;

                bool hasTask = false;
                foreach (var item in m_TaskUIDList)
                {
                    if (!item.Value)
                    {
                        taskUID = item.Key;
                        hasTask = true;
                        break;
                    }
                }

                if (hasTask)
                {
                    m_TaskUIDList[taskUID] = true;
                    return true;
                }
                return false;
            }
        }

        public void DoNextTask()
        {
            if (this.TryGetNextTaskUID(out TKey taskUID))
            {
                this.ExecuteTask(taskUID);
                this.RemoveTaskUID(taskUID);
            }
        }

        protected abstract void ExecuteTask(TKey taskUID);

        protected virtual void RemoveTaskUID(TKey taskUID)
        {
            lock (m_Locker)
            {
                if (m_TaskUIDList.Count == 0)
                {
                    ThreadPool.QueueUserWorkItem(x => { this.RefreshTaskUIDList(); });
                    return;
                }
                if (m_TaskUIDList.ContainsKey(taskUID)) m_TaskUIDList.Remove(taskUID);
            }
        }
    }
}