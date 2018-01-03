using System;

namespace MultipleTaskManager
{
    public interface ITask
    {
        event EventHandler TaskAdded;
        string TaskTypeName { get; }
        int MaxTaskCount { get; }
        int RemainTaskCount { get; }
        void RefreshTaskUIDList();
        void DoNextTask();
    }
}