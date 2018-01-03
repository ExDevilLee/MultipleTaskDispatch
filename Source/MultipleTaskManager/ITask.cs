namespace MultipleTaskManager
{
    public interface ITask
    {
        string TaskTypeName { get; }
        int MaxTaskCount { get; }
        int RemainTaskCount { get; }
        void RefreshTaskUIDList();
        void DoNextTask();
    }
}