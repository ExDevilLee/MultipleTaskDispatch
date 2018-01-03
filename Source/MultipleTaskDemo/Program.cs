using System;
using MultipleTaskManager;

namespace MultipleTaskDemo
{
    class Program
    {
        private static void Main(string[] args)
        {
            TestOfTaskManagerByThread();
            Console.ReadKey();
        }

        private static void TestOfTaskManagerByThread()
        {
            TaskManagerByThread mgr = new TaskManagerByThread(20)
            {
                IntervalOfAutoRefreshSeconds = 10,
                IntervalOfTaskMilliseconds = 200
            };
            mgr.RegisterTask(new TaskOfGuid(5), new TaskOfHashCode(5), new TaskOfTicks(10), new TaskOfCompoundKey(10));
            mgr.Start();
        }
    }
}
