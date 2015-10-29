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
            //if (args.Length != 0)
            //{
            string _id = "2177";
                AutomatedTest BeginTest = new AutomatedTest(_id);
                Console.WriteLine("Тест выполнился! " + DateTime.Now);
            //}
            //else
            //    Console.WriteLine("Not found Run ID!");
            //string str = "2118";
            //AutomatedTest test = new AutomatedTest(str);
            //Console.WriteLine("Тест выполнился! " + DateTime.Now);
        }
    }
}
    

