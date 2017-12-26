using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MultipleTaskManager;

namespace MultipleTaskDemo
{
    public class TaskOfHashCode : BaseTask<string>
    {
        public override string TaskTypeName => nameof(TaskOfHashCode);

        public TaskOfHashCode(int taskCount) : base(taskCount)
        { }

        protected override uint MinRefreshMilliseconds => 3000;

        protected override IList<string> GetTaskUIDList(int top)
        {
            IList<string> list = new List<string>(top);
            for (int i = 0; i < top; i++)
            {
                list.Add($"HashCode_{new object().GetHashCode()}");
            }
            return list;
        }

        protected override void ExecuteTask(string taskUID)
        {
            Random r = new Random();
            System.Threading.Thread.Sleep(r.Next(300, 800));
            Console.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}___{nameof(TaskOfHashCode)}.ExecuteTask, TaskUID is {taskUID}. Thread id is {System.Threading.Thread.CurrentThread.ManagedThreadId}.");
        }
    }
}