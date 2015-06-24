using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Opera;
using OpenQA.Selenium.Safari;
using OpenQA.Selenium.Support;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutotestDesktop
{
	public class AutomatedTest
	{
        private TestRail testrail;
        private Driver Browser;
        private string _createTest = "";
        private string _deleteTest = "";
        public AutomatedTest()
        {
            testrail = new TestRail();
            //testrail.CreateSuite();
            //testrail.UpdateTestSuite("117", Console.ReadLine());
            //return;
          //  testrail.AddCases();

            testrail.GetSuitesOfProject();
            testrail.GetRunsProject();

            //Console.WriteLine("Do your want regular (reg) test or FULL (full) test ?");
            //Console.Write("Do you want to delete a test-run? _");
            //_deleteTest = Console.ReadLine();
            //DoYoyWantDeleteTest();
            Console.Write("Do you want create a test-run (y/n) _");
            _createTest = Console.ReadLine();
            Browser = new Driver (testrail);
            DoYouWantCreateTest();

            //FireFoxOnClick();
            ChromeOnClick();
            //OperaOnClick();
            //IEOnClick();
            //SafariOnClick();
        }
        
        private void DoYouWantCreateTest()
        {
            if (_createTest == "y")
            {
                Console.WriteLine("\nPlease, enter name of run-test & SuitesID:");
                Console.Write("Name _");
                string nameSuite = Console.ReadLine();
                Console.Write("SuitesID _");
                string suiteID = Console.ReadLine();
                testrail.CreateRun(testrail.GetSuiteID = suiteID, nameSuite);
            }
            else
            {
                Console.Write("Input run ID _");
                testrail.RunID = Console.ReadLine();
                Console.Write("Input suite ID _");
                testrail.GetSuiteID = Console.ReadLine();
                Console.WriteLine("Test is running...");
            }
        }
        private void DoYoyWantDeleteTest()
        {
            while (_deleteTest == "y")
            {
                Console.Write("Please, input run ID: ");
                testrail.DeleteRun(Console.ReadLine());
                testrail.GetSuitesOfProject();
                testrail.GetRunsProject();
                Console.Write("Do you want to delete again? _");
                _deleteTest = Console.ReadLine();
            }
        }        
		private void FireFoxOnClick()
		{   
			RunBrowser(new FirefoxDriver());
		}
		private void ChromeOnClick()
		{              
            RunBrowser(new ChromeDriver());
		}
        private void OperaOnClick()
		{
            RunBrowser(new OperaDriver());
		
		}
        private void IEOnClick()
		{
            RunBrowser(new InternetExplorerDriver());
			
		}
        private void SafariOnClick()
		{
            RunBrowser(new SafariDriver());
		}
        private void RunBrowser(IWebDriver webDriver)
        {
            Browser.Drivers.Add(webDriver);
            Browser.NavigateDriver(webDriver);
            webDriver.Quit();
        }
    }
     
}
