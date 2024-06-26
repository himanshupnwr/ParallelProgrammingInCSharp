﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DataSharingAndSync
{
    public class MutexExample
    {
        static void Main(string[] args)
        {
            LocalMutex();

            GlobalMutex();

            Console.WriteLine("All done here");
        }

        private static void GlobalMutex()
        {
            const string appName = "MyApp";
            Mutex mutex;
            try
            {
                mutex = Mutex.OpenExisting(appName);
                Console.WriteLine($"Sorry, {appName} is already running");
                return;
            }
            catch (WaitHandleCannotBeOpenedException e)
            {
                Console.WriteLine("We can run the program just fine.");
                // first arg = whether to give current thread initial ownership
                mutex = new Mutex(false, appName);
            }
            Console.ReadKey();
        }

        private static void LocalMutex()
        {
            var tasks = new List<Task>();
            var bankAccount1 = new MutexBankAccount(0);
            var bankAccount2 = new MutexBankAccount(0);

            // many synchro types deriving from WaitHandle
            // Mutex = mutual exclusion

            // two types of mutexes
            // this is a _local_ mutex
            Mutex mutex = new Mutex();
            Mutex mutex2 = new Mutex();

            for (int i = 0; i < 10; ++i)
            {
                tasks.Add(Task.Factory.StartNew(() =>
                {
                    for (int j = 0; j < 1000; ++j)
                    {
                        bool haveLock = mutex.WaitOne();
                        try
                        {
                            bankAccount1.Deposit(1); //deposit 10000 overall
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                            throw;
                        }
                        finally
                        {
                            if (haveLock)
                                mutex.ReleaseMutex();
                        }
                    }
                }));
                tasks.Add(Task.Factory.StartNew(() =>
                {
                    for (int j = 0; j < 1000; ++j)
                    {
                        bool haveLock = mutex2.WaitOne();
                        try
                        {
                            bankAccount2.Deposit(1); // deposit 10000 overall
                        }
                        finally
                        {
                            if (haveLock) mutex2.ReleaseMutex();
                        }
                    }
                }));
                // transfer needs to lock both accounts
                tasks.Add(Task.Factory.StartNew(() =>
                {
                    for (int j = 0; j < 1000; j++)
                    {
                        bool haveLock = Mutex.WaitAll(new[] {mutex, mutex2});
                        try
                        {
                            bankAccount1.Transfer(bankAccount2, 1); // transfer 10k from ba to ba2
                        }
                        finally
                        {
                            if (haveLock)
                            {
                                mutex.ReleaseMutex();
                                mutex2.ReleaseMutex();
                            }
                        }
                    }
                }));
            }

            Task.WaitAll(tasks.ToArray());

            Console.WriteLine($"Final balance is: ba={bankAccount1.Balance}, ba2={bankAccount2.Balance}.");
        }

        internal class MutexBankAccount
        {
            public int Balance { get; private set; }

            public MutexBankAccount(int balance)
            {
                Balance = balance;
            }

            public void Deposit(int amount)
            {
                Balance += amount;
            }

            public void Withdraw(int amount)
            {
                Balance -= amount;
            }

            public void Transfer(MutexBankAccount where, int amount)
            {
                where.Balance += amount;
                Balance -= amount;
            }
        }
    }
}
