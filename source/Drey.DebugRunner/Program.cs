using System;

namespace Drey.DebugRunner
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var horde = new Horde())
            {
                horde.Startup();
                Console.WriteLine("Hoarde is online.");
                Console.ReadLine();
            }
        }
    }
}