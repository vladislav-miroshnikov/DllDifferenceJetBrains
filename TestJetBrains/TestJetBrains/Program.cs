using System;
using System.IO;

namespace TestJetBrains
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("You have entered insufficient number of arguments\n The number of input arguments must be two");
                return;
            }

            if (!File.Exists(args[0]))
            {
                Console.WriteLine($"The assembly along the path {args[0]} does not exist. Check the entered path");
                return;
            }

            OperationConverter.ReplaceAdditOp(args[0], args[1]);
        }
    }
}
