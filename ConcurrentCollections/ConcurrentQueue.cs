using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConcurrentCollections
{
    public class ConcurrentQueue
    {
        static void Main(string[] args)
        {
            var qu = new ConcurrentQueue<int>();
            qu.Enqueue(1);
            qu.Enqueue(2);

            //Queue 2 1

            int result;
            //int last = qu.Dequeue();

            if (qu.TryDequeue(out result))
            {
                Console.WriteLine($"Removed Element {result}");
            }

            //Queue 2

            //int peeked = qu.Peek();
            if (qu.TryPeek(out result))
            {
                Console.WriteLine($"Last element is {result}");
            }
        }
    }
}

/*Removed Element 1
Last element is 2*/
