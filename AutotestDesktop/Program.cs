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
                List<string> testIDs = new List<string>();
                Console.Write("Run ID: ");
                string runID = Console.ReadLine();
                Console.Write("Test ID: ");
                string test_id = Console.ReadLine();
                testIDs.Add(test_id);
                AutomatedTest BeginTest = new AutomatedTest(runID, testIDs);         
        }
    }
}
    

