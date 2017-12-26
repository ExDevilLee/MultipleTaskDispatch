using System;
using MultipleTaskManager;

namespace MultipleTaskDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            TaskManager mgr = new TaskManager(20);
            mgr.RegisterTask(new TaskOfGuid(5));
            mgr.RegisterTask(new TaskOfHashCode(5));
            mgr.RegisterTask(new TaskOfTicks(10));
            mgr.Start();

            Console.ReadKey();
        }
    }
}
