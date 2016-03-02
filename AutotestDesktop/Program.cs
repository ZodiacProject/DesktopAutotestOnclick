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
                List<string> testIDs = new List<string>();
                string[] DataStr = args[0].Split('#');
                string runID = DataStr[0];
                testIDs = DataStr[1].Split(',').ToList();
                Console.WriteLine(runID + "\n");
                AutomatedTest BeginTest = new AutomatedTest(runID, testIDs);
                Console.WriteLine("Тест выполнился! " + DateTime.Now);
            }
            else
                Console.WriteLine("You don't chosen ids from test run!");     
        }
    }
}
    

