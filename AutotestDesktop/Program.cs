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
            Console.WriteLine("Please, input Name of Run and SuiteID");
            string nameSuite = Console.ReadLine();
            int suiteID = int.Parse(Console.ReadLine());
            TestRail testrail = new TestRail();
            testrail.CreateRun(suiteID, nameSuite);
           //testrail.CloseRun();
        }
    }
}
    

