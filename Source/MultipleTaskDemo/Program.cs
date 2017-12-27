using System;
using MultipleTaskManager;

namespace MultipleTaskDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            TaskManager mgr = new TaskManager(20)
            {
                IntervalOfAutoRefreshSeconds = 30,
                IntervalOfTaskMilliseconds = 200
            };
            mgr.RegisterTask(new TaskOfGuid(5), new TaskOfHashCode(5), new TaskOfTicks(10), new TaskOfCompoundKey(10));
            mgr.Start();

            Console.ReadKey();
        }
    }
}
