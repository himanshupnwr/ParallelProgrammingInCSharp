using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataSharingAndSync
{
    class CriticalSections
    {
        static void Main(string[] args)
        {
            var tasks = new List<Task>();
            var bankObject = new BankAccount();

            for (int i = 0; i < 10; ++i)
            {
                tasks.Add(Task.Factory.StartNew(() =>
                {
                    for (int j = 0; j < 1000; ++j)
                    {
                        bankObject.Deposit(100);
                    }
                }));
                tasks.Add(Task.Factory.StartNew(() =>
                {
                    for (int j = 0; j < 1000; ++j)
                        bankObject.Withdraw(100);
                }));
            }

            Task.WaitAll(tasks.ToArray());

            Console.WriteLine($"Final Balance is bankObject.Balance");
            Console.WriteLine("All done here");
        }
    }

    internal class BankAccount
    {
        public object padlock = new object();
        public int Balance { get; private set; }

        internal void Deposit(int amount)
        {
            //critical section - critical section is basically a piece of code or a marker on the piece of code,
            //which says that only one thread can enter this particular area
            lock (padlock)
            {
                // += is really two operations
                // op1 is temp <- get_Balance() + amount
                // op2 is set_Balance(temp)
                // something can happen _between_ op1 and op2
                Balance += amount;
            }
        }

        internal void Withdraw(int amount)
        {
            lock (padlock)
            {
                Balance -= amount;
            }
        }
    }
}
