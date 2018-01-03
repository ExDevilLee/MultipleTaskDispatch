using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MultipleTaskManager;

namespace MultipleTaskDemo
{
    public struct CompoundKey
    {
        public string UID { get; }
        public string TypeCode { get; }

        public CompoundKey(string uid, string typeCode)
        {
            this.UID = uid;
            this.TypeCode = typeCode;
        }

        public override bool Equals(object obj)
        {
            if (obj is CompoundKey)
            {
                CompoundKey other = (CompoundKey)obj;
                return this.UID == other.UID && this.TypeCode == other.TypeCode;
            }
            return false;
        }
        public override int GetHashCode()
        {
            unchecked
            {
                return ((UID?.GetHashCode() ?? 0) * 397) ^ (TypeCode?.GetHashCode() ?? 0);
            }
        }
        public override string ToString()
        {
            return $"UID={this.UID},TypeCode={this.TypeCode}";
        }
    }
    public class TaskOfCompoundKey : BaseTask<CompoundKey>
    {
        public override string TaskTypeName => nameof(TaskOfCompoundKey);

        public TaskOfCompoundKey(int maxTaskCount) : base(maxTaskCount)
        { }

        protected override uint AutoRefreshTriggerCount => 2;

        protected override IList<CompoundKey> GetTaskUIDList(int top)
        {
            System.Threading.Thread.Sleep(3000);

            Random r = new Random();
            string[] typeCodes = { "101", "102", "103", "104", "105" };

            IList<CompoundKey> list = new List<CompoundKey>(top);
            for (int i = 0; i < top; i++)
            {
                var ck = new CompoundKey(uid: Guid.NewGuid().ToString("N").ToUpper().Substring(0, 16),
                    typeCode: typeCodes[r.Next(0, typeCodes.Length)]);
                list.Add(ck);
            }
            return list;
        }

        protected override void ExecuteTask(CompoundKey taskUID)
        {
            Random r = new Random();
            System.Threading.Thread.Sleep(r.Next(2500, 5000));
            Console.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}___{nameof(TaskOfCompoundKey)}.ExecuteTask, TaskUID is {taskUID}. Thread id is {System.Threading.Thread.CurrentThread.ManagedThreadId}.");
        }
    }
}