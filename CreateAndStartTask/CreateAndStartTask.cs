using System;
using System.Threading.Tasks;

namespace CreateAndStartTask
{
    class CreateAndStartTask
    {
        static void Main(string[] args)
        {
            //CreateAndStartSimpleTasks();
            //TasksWithState();
            TasksWithReturnValues();

            Console.WriteLine("Main Program Done, press any key");
        }

        private static void TasksWithReturnValues()
        {
            string text1 = "testing", text2 = "this";
            var task1 = new Task<int>(TextLength, text1);
            task1.Start();

            var task2 = Task.Factory.StartNew(TextLength, text2);
            //getting the result is a blocking operation
            Console.WriteLine($"Length of '{text1}' is {task1.Result}.");
            Console.WriteLine($"Length of '{text2}' is {task2.Result}.");
        }

        private static int TextLength(object arg)
        {
            Console.WriteLine($"\n Task with id {Task.CurrentId} processing object '{arg}' ...");
            return arg.ToString().Length;
        }
    }

    class IntroducingTasks
    {
        public static void Write(char c)
        {
            int i = 1000;
            while (i-- > 0)
            {
                Console.Write(c);
            }
        }

        public static void Write(object s)
        {
            int i = 1000;
            while (i-- > 0)
            {
                Console.Write(s.ToString());
            }
        }

        public static void CreateAndStartSimpleTasks()
        {
            //A Task is a unit of work in .net

            //here is how we make a simple task that does something, it starts automatically
            Task.Factory.StartNew(() =>
            {
                //Console.WriteLine("Hello, Tasks!");
                Write('-');
            });

            //the arguement is an action, so it can be a delegate, a lambda or an anonymous method 

            Task t = new Task(() => Write('?'));
            t.Start(); // task doesn't start automatically
        }

        private static void TasksWithState()
        {
            //clumsy 'object approach'
            Task t = new Task(Write, "foo");
            t.Start();
            Task.Factory.StartNew(Write, "bar");
        }

        // Summary:

        // 1. Two ways of using tasks
        //    Task.Factory.StartNew() creates and starts a Task
        //    new Task(() => { ... }) creates a task; use Start() to fire it
        // 2. Tasks take an optional 'object' argument
        //    Task.Factory.StartNew(x => { foo(x) }, arg);
        // 3. To return values, use Task<T> instead of Task
        //    To get the return value. use t.Result (this waits until task is complete)
        // 4. Use Task.CurrentId to identify individual tasks.

    }

}
