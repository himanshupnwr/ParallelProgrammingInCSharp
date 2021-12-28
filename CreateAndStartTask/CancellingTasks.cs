using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CreateAndStartTask
{
    class CancellingTasks
    {
        static void main(string[] args)
        {
            CancelableTasks();
            MonitoringCancellation();
            CompositeCancellationToken();

            Console.WriteLine("Main Program Done, press any key");
            Console.ReadKey();
        }

        private static void WaitingForTimePass()
        {
            // we have already seen the classic Thread.Sleep

            var cts = new CancellationTokenSource();
            var token = cts.Token;
            var t = new Task(() =>
            {
                Console.WriteLine("You have 5 seconds to disarm this bomb by pressing a key");
                bool cancelled = token.WaitHandle.WaitOne(5000);
                Console.WriteLine(cancelled ? "Bomb Disarmed" : "BOOM!!!");
            }, token);
            t.Start();

            //unlike sleep and waitone
            //thread does not give up its turn

            Thread.SpinWait(10000);
            Console.WriteLine("Are you still here?");

            Console.ReadKey();
            cts.Cancel();
        }

        private static void CompositeCancellationToken()
        {
           //its possible to create a composite cancellation source that involves several tokens
           var planned = new CancellationTokenSource();
           var preventative = new CancellationTokenSource();
           var emergency = new CancellationTokenSource();

           //make a token source that is linked in their tokens
           var paranoid =
               CancellationTokenSource.CreateLinkedTokenSource(planned.Token, preventative.Token, emergency.Token);

           Task.Factory.StartNew(() =>
           {
               int i = 0;
               while (true)
               {
                   paranoid.Token.ThrowIfCancellationRequested();
                   Console.WriteLine($"{i++} \t");
                   Thread.Sleep(100);
               }
           }, paranoid.Token);

           paranoid.Token.Register(() => Console.WriteLine("Cancellation Requested"));

           Console.ReadKey();

           //use any of the aformentioned token sources
           emergency.Cancel();
        }

        private static void MonitoringCancellation()
        {
            var cts = new CancellationTokenSource();
            var token = cts.Token;

            //register a delegate to fire
            token.Register(() => Console.WriteLine("Cancellation has been requested"));

            Task t = new Task(() =>
            {
                int i = 0;
                while (true)
                {
                    if (token.IsCancellationRequested) // soft exit
                    {
                        break;
                    }
                    else
                    {
                        Console.Write($"{i++}\t");
                        Thread.Sleep(100);
                    }
                }
            });
            t.Start();

            //cancelling multiple tasks
            Task t2 = Task.Factory.StartNew(() =>
            {
                char c = 'a';
                while (true)
                {
                    //alternative to whats below
                    token.ThrowIfCancellationRequested(); // hard exit

                    if (token.IsCancellationRequested)
                    {
                        throw new OperationCanceledException("No longer interested in printing letters");
                    }
                    else
                    {
                        Console.Write($"{c++} \t");
                        Thread.Sleep(200);
                    }
                }
            }, token);

            //cancellation in a wait handle
            Task.Factory.StartNew(() =>
            {
                token.WaitHandle.WaitOne();
                Console.WriteLine("Wait handle released, thus cancellation was requested");
            });

            Console.ReadKey();
            cts.Cancel();
            Thread.Sleep(1000); //cancellation is non instant

            Console.WriteLine($"Task has been canceled. The status of the canceled task 't' is {t.Status}.");
            Console.WriteLine($"Task has been canceled. The status of the canceled task 't2' is {t2.Status}.");
            Console.WriteLine($"t.IsCanceled = {t.IsCanceled}, t2.IsCanceled = {t2.IsCanceled}");
        }

        public static void CancelableTasks()
        {
            var cts = new CancellationTokenSource();
            var token = cts.Token;
            Task tt = new Task(() =>
            {
                int i = 0;
                while (true)
                {
                    if (token.IsCancellationRequested) //task completion is cooperative, no one kills your thread
                        break;
                    else
                    {
                        Console.WriteLine($"{i++} \t");
                    }
                }
            });
            tt.Start();

            //don't forget CancellationToken.None
            Console.ReadKey();
            cts.Cancel();
            Console.WriteLine("Task has been cancelled");
        }

    }
}
