using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DebugTarget
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("DebugTarget Started");
            Console.WriteLine("Calling Debugger.IsAttached");
            Console.WriteLine(Debugger.IsAttached);
        }
    }
}
