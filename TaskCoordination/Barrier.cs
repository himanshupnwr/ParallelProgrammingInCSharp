using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TaskCoordination
{
    public class BarrierDemo
    {
        static Barrier barrier = new Barrier(2, b =>
        {
            Console.WriteLine($"Phase {b.CurrentPhaseNumber} is finished.");
            //b.ParticipantCount
            //b.ParticipantsRemaining

            // add/remove participants
        });

        private static void Water()
        {
            Console.WriteLine("Putting the kettle on (takes a bit longer)");
            Thread.Sleep(2000);
            barrier.SignalAndWait(); //signaling the waiting fused
            Console.WriteLine("Putting water into cup");
            barrier.SignalAndWait();
            Console.WriteLine("Putting the cattle away");
        }

        private static void Cup()
        {
            Console.WriteLine("Finding the nicest cup of tea (only takes a second)");
            barrier.SignalAndWait();
            Console.WriteLine("Adding tea");
            barrier.SignalAndWait();
            Console.WriteLine("Adding sugar");
        }

        static void Main_Barrier(string[] args)
        {
            var water = Task.Factory.StartNew(Water);
            var cup = Task.Factory.StartNew(Cup);

            var tea = Task.Factory.ContinueWhenAll(new[] { water, cup }, tasks =>
              {
                  Console.WriteLine("Enjoy your cup of tea");
              });

            tea.Wait();
        }
    }
}

/*Output
    Putting the kettle on (takes a bit longer)
    Finding the nicest cup of tea (only takes a second)
    Phase 0 is finished.
    Putting water into cup
    Adding tea
    Phase 1 is finished.
    Adding sugar
    Putting the cattle away
    Enjoy your cup of tea
*/