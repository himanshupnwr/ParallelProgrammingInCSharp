using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DataSharingAndSync
{
    // spinning avoid overhead of resheduling
    // useful if you expect the wait time to be very short
    public class SpinLocking
    {
        public static void SpinLockDemo()
        {
            var tasks = new List<Task>();
            var bankAccout = new SpinBankAccount();

            //spinning avoid overhead of rescheduling, useful if you expect the wait time to be very short

            SpinLock spin = new SpinLock();

            //owner tracking keeps a record of which thread acquired it to improve debugging

            for (int i = 0; i < 10; ++i)
            {
                tasks.Add(Task.Factory.StartNew(() =>
                {
                    for (int j = 0; j < 1000; ++j)
                    {
                        bool lockTaken = false;
                        try
                        {
                            //spin.IsHeld;
                            //spin.IsHeldByCurrentThread
                            spin.Enter(ref lockTaken);
                            bankAccout.Deposit(100);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                            throw;
                        }
                        finally
                        {
                            if (lockTaken) spin.Exit();
                        }
                    }
                }));
                tasks.Add(Task.Factory.StartNew(() =>
                {
                    for (int j = 0; j < 1000; ++j)
                    {
                        bool lockTaken = false;
                        try
                        {
                            //spin.IsHeld;
                            //spin.IsHeldByCurrentThread
                            spin.Enter(ref lockTaken);
                            bankAccout.Withdraw(100);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                            throw;
                        }
                        finally
                        {
                            if (lockTaken) spin.Exit();
                        }
                    }
                }));
            }

            Task.WaitAll(tasks.ToArray());
            Console.WriteLine($"Final balance is {bankAccout.Balance}.");

        }

        class SpinBankAccount
        {
            public int Balance { get; private set; }

            public void Deposit(int amount)
            {
                Balance += amount;
            }

            public void Withdraw(int amount)
            {
                Balance -= amount;
            }
        }

        static void Main(string[] args)
        {
            SpinLockDemo();

            LockRecursion(5);

            Console.ReadKey();
            Console.WriteLine("All done here.");
        }

        // true = exception, false = deadlock
        static SpinLock spin2 = new SpinLock(true);
        private static void LockRecursion(int x)
        {
            //lock recursion is being able to take the same lock multiple times, which is a problem and should be avoided
            bool lockTaken = false;

            try
            {
                spin2.Enter(ref lockTaken);
            }
            catch (LockRecursionException e)
            {
                Console.WriteLine("Exception:" + e);
            }
            finally
            {
                if (lockTaken)
                {
                    Console.WriteLine($"Took a lock, x = {x}");
                    LockRecursion(x-1);
                    spin2.Exit();
                }
                else
                {
                    Console.WriteLine($"Failed to take a lock, x = {x}");
                }
            }
        }
    }
}
