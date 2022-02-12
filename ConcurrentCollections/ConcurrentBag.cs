using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConcurrentCollections
{
    public class ConcurrentBag
    {
        static void Main(string[] args)
        {
            //stack is LIFO
            //queue is FIFO
            //concurrent bag does not provide any ordering guarantee
            //keeps a separate list of items for each thread
            //typically requires no synchronization, unless a thread tries to remove an item
            //while the thread-local bag is empty (item stealing)

            var bag = new ConcurrentBag<int>();
            var tasks = new List<Task>();
            for (int i = 0; i < 10; i++)
            {
                var i1 = i;
                tasks.Add(Task.Factory.StartNew(() =>
                    {
                        bag.Add(i1);
                        Console.WriteLine($"{Task.CurrentId} has been added {i1}");
                        int result;
                        if (bag.TryPeek(out result))
                        {
                            Console.WriteLine($"{Task.CurrentId} has peeked the value {result}");
                        }
                    }));
            }

            Task.WaitAll(tasks.ToArray());

            //take whatever's last
            int last;
            if (bag.TryTake(out last))
            {
                Console.WriteLine($"Last element is {last}");
            }
        }
    }
}

/*4 has been added 0
5 has been added 1
8 has been added 3
1 has been added 6
3 has been added 4
3 has peeked the value 4
2 has been added 7
2 has peeked the value 7
1 has peeked the value 6
4 has peeked the value 0
5 has peeked the value 1
8 has peeked the value 3
6 has been added 2
7 has been added 5
10 has been added 9
9 has been added 8
9 has peeked the value 8
7 has peeked the value 5
10 has peeked the value 9
6 has peeked the value 2
Last element is 3*/