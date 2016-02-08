using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gurock.TestRail;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace AutotestDesktop
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 0)
            {
                AutomatedTest BeginTest = new AutomatedTest(args[0]);
                Console.WriteLine("Тест выполнился! " + DateTime.Now);
            }
            else
                Console.WriteLine("Not found Plan ID!");    
        }
    }
}
    

