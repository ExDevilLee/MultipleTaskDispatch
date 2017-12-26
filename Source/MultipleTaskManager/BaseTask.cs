using System.Collections.Generic;
using System.Threading;

namespace MultipleTaskManager
{
    public abstract class BaseTask<TKey> : ITask
    {
        private readonly object m_Locker = new object();
        private IDictionary<TKey, bool> m_TaskUIDList = new Dictionary<TKey, bool>();
        private static bool TKeyIsValueType => typeof(TKey).IsValueType;

        public abstract string TaskTypeName { get; }
        public int TaskCount { get; }

        protected BaseTask(int taskCount)
        {
            this.TaskCount = taskCount;
            ThreadPool.QueueUserWorkItem(x => { this.RefreshTaskUIDList(); });
        }

        protected abstract IList<TKey> GetTaskUIDList(int top);

        public void RefreshTaskUIDList()
        {
            IList<TKey> taskList = this.GetTaskUIDList(this.TaskCount);
            if (null == taskList || taskList.Count == 0) return;

            lock (m_Locker)
            {
                foreach (var uid in taskList)
                {
                    if (!TKeyIsValueType && null == uid) continue;
                    if (m_TaskUIDList.ContainsKey(uid)) continue;
                    m_TaskUIDList.Add(uid, false);
                    if (m_TaskUIDList.Count >= this.TaskCount) break;
                }
            }
        }

        protected virtual int AutoRefreshTriggerCount => 1;
        protected virtual bool TryGetNextTaskUID(out TKey taskUID)
        {
            taskUID = default(TKey);
            lock (m_Locker)
            {
                if (m_TaskUIDList.Count <= this.AutoRefreshTriggerCount)
                {
                    ThreadPool.QueueUserWorkItem(x => { this.RefreshTaskUIDList(); });
                }
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
                if (m_TaskUIDList.Count == 0) return;
                if (m_TaskUIDList.ContainsKey(taskUID)) m_TaskUIDList.Remove(taskUID);
            }
        }
    }
}