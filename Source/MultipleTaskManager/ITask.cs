namespace MultipleTaskManager
{
    public interface ITask
    {
        string TaskTypeName { get; }
        void RefreshTaskUIDList();
        void DoNextTask();
    }
}