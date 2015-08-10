using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drey.Client
{
    class Program
    {
        static object[] _commands = { new Commands.DeleteCommand(), new Commands.PushCommand() };
        static void Main(string[] args)
        {
        }
    }
}
