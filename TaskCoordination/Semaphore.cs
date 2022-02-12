using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TaskCoordination
{
    public class SemaphoreDemo
    {
        static void Main()
        {
            var semaphore = new SemaphoreSlim(2, 10);

            for (int i = 0; i < 20; ++i)
            {
                Task.Factory.StartNew(() =>
                {
                    Console.WriteLine($"Entering Task {Task.CurrentId}");
                    semaphore.Wait();
                    Console.WriteLine($"Processing task {Task.CurrentId}");
                });
            }

            while (semaphore.CurrentCount <=2)
            {
                Console.WriteLine($"Semaphore count: {semaphore.CurrentCount}");
                Console.ReadKey();
                semaphore.Release(2); //ReleaseCount +=n
            }
        }
    }
}

/*
 * Output
 * Entering Task 4
   Entering Task 6
   Entering Task 11
   Entering Task 15
   Semaphore count: 2
   Entering Task 7
   Entering Task 16
   Entering Task 5
   Entering Task 2
   Entering Task 8
   Entering Task 3
   Entering Task 9
   Entering Task 10
   Processing task 4
   Processing task 6
   Entering Task 12
   Entering Task 13
   Entering Task 14
   Entering Task 1
   Entering Task 18
   Entering Task 17
   Entering Task 19
   Entering Task 20
   Semaphore count: 2
   Processing task 15
   Processing task 11
   Semaphore count: 2
   Processing task 16
   Processing task 7
   Semaphore count: 2
   Processing task 5
   Processing task 2
   Semaphore count: 2
   Processing task 3
   Processing task 8
   Semaphore count: 2
   Processing task 9
   Processing task 10
   Semaphore count: 2
   Processing task 12
   Processing task 13
   Semaphore count: 2
   Processing task 14
   Processing task 1
   Semaphore count: 2
   Processing task 17
   Processing task 18
   Semaphore count: 2
   Processing task 19
   Processing task 20
   Semaphore count: 2
 */
