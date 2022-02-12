using System;
using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace ConcurrentCollections
{
    public class ConcurrentDictionary
    {
        private static ConcurrentDictionary<string, string> capitals = new ConcurrentDictionary<string, string>();

        public static void AddParis()
        {
            //there is no add, since you don't know if it will succeed
            bool success = capitals.TryAdd("France", "Paris");
            string whoCalled = Task.CurrentId.HasValue ? ("Task" + Task.CurrentId) : "Main Thread";
            Console.WriteLine($"{whoCalled} {(success ? "added" : "did not add")} the element");
        }
        static void Main(string[] args)
        {
            Task.Factory.StartNew(AddParis).Wait();
            AddParis();

            capitals["Russia"] = "Leningrad";
            var newCapital = capitals.AddOrUpdate("Russia", "Moscow", (k, old) => old + " --> Moscow");
            Console.WriteLine("The capital of Russia is " + capitals["Russia"]);

            capitals["Sweden"] = "Uppsala";
            var capOfSweden = capitals.GetOrAdd("Sweden", "Stockholm");
            Console.WriteLine($"The capital of Sweden is {capOfSweden}.");

            //removing
            const string toRemove = "Russia"; //make a mistake here
            string removed;
            var didRemove = capitals.TryRemove(toRemove, out removed);
            if (didRemove)
            {
                Console.WriteLine($"We just removed {removed}");
            }
            else
            {
                Console.WriteLine($"Failed to remove capital of {toRemove}");
            }

            //some operations are slow
            Console.WriteLine($"The");
            foreach (var kv in capitals)
            {
                Console.WriteLine($" - {kv.Value} is the capital of {kv.Key}");
            }

            Console.ReadKey();
        }
    }
}

/*
   Task1 added the element
   Main Thread did not add the element
   The capital of Russia is Leningrad --> Moscow
   The capital of Sweden is Uppsala.
   We just removed Leningrad --> Moscow
   The
   - Uppsala is the capital of Sweden
   - Paris is the capital of France


   Task1 added the element
   Main Thread did not add the element
   The capital of Russia is Moscow
   The capital of Sweden is Uppsala.
   We just removed Moscow
   The
   - Paris is the capital of France
   - Uppsala is the capital of Sweden*/
