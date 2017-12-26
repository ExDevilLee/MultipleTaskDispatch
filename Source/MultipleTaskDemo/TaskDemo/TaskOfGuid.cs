using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MultipleTaskManager;

namespace MultipleTaskDemo
{
    public class TaskOfGuid : BaseTask<string>
    {
        public override string TaskTypeName => nameof(TaskOfGuid);

        public TaskOfGuid(int taskCount) : base(taskCount)
        { }

        protected override IList<string> GetTaskUIDList(int top)
        {
            IList<string> list = new List<string>(top);
            for (int i = 0; i < top; i++)
            {
                list.Add($"Guid_{Guid.NewGuid().ToString("N").ToUpper()}");
            }
            return list;
        }

        protected override void ExecuteTask(string taskUID)
        {
            Random r = new Random();
            System.Threading.Thread.Sleep(r.Next(100, 500));
            Console.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}___{nameof(TaskOfGuid)}.ExecuteTask, TaskUID is {taskUID}. Thread id is {System.Threading.Thread.CurrentThread.ManagedThreadId}.");
        }
    }
}