using System;
using MultipleTaskManager;

namespace MultipleTaskDemo
{
    class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine($"Welcome to the {nameof(MultipleTaskDemo)}!");

            Start:
            Console.WriteLine();
            Console.WriteLine("There have some different types:");
            Console.WriteLine("Index\tType");
            Console.WriteLine($"  1\t{nameof(TaskManagerByThread)}");
            Console.WriteLine($"  2\t{nameof(TaskManagerByThreadPool)}");
            Console.WriteLine();
            Console.WriteLine("Please enter index of type to start. (Or enter 'Esc' to exit.)");

            ITask[] task = { new TaskOfGuid(10), new TaskOfHashCode(10), new TaskOfTicks(10), new TaskOfCompoundKey(10) };
            var enterKey = Console.ReadKey().Key;
            switch (enterKey)
            {
                case ConsoleKey.D1:
                case ConsoleKey.NumPad1:
                    Console.Clear();
                    Console.WriteLine($"Type is {nameof(TaskManagerByThread)}:");
                    TestOfTaskManagerByThread(task);
                    break;
                case ConsoleKey.D2:
                case ConsoleKey.NumPad2:
                    Console.Clear();
                    Console.WriteLine($"Type is {nameof(TaskManagerByThreadPool)}:");
                    TestOfTaskManagerByThreadPool(task);
                    break;
                case ConsoleKey.Escape:
                    break;
                default:
                    Console.Clear();
                    Console.WriteLine($"Opps! I don't know what your choice :'{enterKey}'. Please try again!");
                    goto Start;
            }
        }

        private static void TestOfTaskManagerByThread(ITask[] task)
        {
            TaskManagerByThread mgr = new TaskManagerByThread(20)
            {
                IntervalOfAutoRefreshSeconds = 10,
                IntervalOfTaskMilliseconds = 200
            };
            mgr.RegisterTask(task);
            mgr.Start();
            Console.ReadKey();
        }

        private static void TestOfTaskManagerByThreadPool(ITask[] task)
        {
            TaskManagerByThreadPool mgr = new TaskManagerByThreadPool(10)
            {
                IntervalOfAutoRefreshSeconds = 10
            };
            mgr.RegisterTask(task);
            mgr.Start();
            Console.ReadKey();
        }
    }
}