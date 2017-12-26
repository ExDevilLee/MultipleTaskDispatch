using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MultipleTaskManager;

namespace MultipleTaskDemo
{
    public class TaskOfTicks : BaseTask<string>
    {
        public override string TaskTypeName => nameof(TaskOfTicks);

        public TaskOfTicks(int taskCount) : base(taskCount)
        { }

        protected override int AutoRefreshTriggerCount => 2;
        protected override IList<string> GetTaskUIDList(int top)
        {
            IList<string> list = new List<string>(top);
            for (int i = 0; i < top; i++)
            {
                list.Add($"Ticks_{DateTime.Now.Ticks}");
            }
            return list;
        }

        protected override void ExecuteTask(string taskUID)
        {
            Random r = new Random();
            System.Threading.Thread.Sleep(r.Next(500, 1500));
            Console.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}___{nameof(TaskOfTicks)}.ExecuteTask, TaskUID is {taskUID}. Thread id is {System.Threading.Thread.CurrentThread.ManagedThreadId}.");
        }
    }
}