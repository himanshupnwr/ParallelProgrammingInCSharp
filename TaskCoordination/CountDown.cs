using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TaskCoordination
{
    public class CountDown
    {
        private static int taskCount = 5;
        private static CountdownEvent cte = new CountdownEvent(taskCount);
        private static Random random = new Random();

        static void Main_Countdown(string[] args)
        {
            var tasks = new Task[taskCount];
            for(int i=0; i < taskCount; i++)
            {
                tasks[i] = Task.Factory.StartNew(() =>
                {
                    Console.WriteLine($"Entering task {Task.CurrentId}");
                    Thread.Sleep(random.Next(3000));
                    cte.Signal(); // also takes a signal count
                    //cte.CurrentCount/Initial count
                    Console.WriteLine($"Exiting task {Task.CurrentId}");
                });
            }

            var finalTask = Task.Factory.StartNew(() =>
            {
                Console.WriteLine(($"Waiting for other tasks in task {Task.CurrentId}"));
                cte.Wait();
                Console.WriteLine("All tasks completed");
            });

            finalTask.Wait();
        }
    }
}

/*Output
 Waiting for other tasks in task 1
   Entering task 2
   Entering task 3
   Entering task 4
   Entering task 5
   Entering task 6
   Exiting task 6
   Exiting task 3
   Exiting task 2
   Exiting task 5
   Exiting task 4
   All tasks completed
 */
