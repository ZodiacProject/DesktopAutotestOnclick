using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Opera;
using OpenQA.Selenium.Safari;
using OpenQA.Selenium.Support;
using OpenQA.Selenium.Remote;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutotestDesktop
{
	public class AutomatedTest
	{
        private TestRail _testrail;
        private Driver _browsers;
        private string _addSuite = "";
        private string _createTest = "";
        private string _deleteTest = "";
        public AutomatedTest()
        {
            _testrail = new TestRail();
            _testrail.GetSuitesOfProject();
            _testrail.GetRunsProject();
            Console.Write("Do you want to create a new Top sites suites (y/n)_");
            _addSuite = Console.ReadLine();
            _doAddSuite();
            Console.Write("Do you want create a test-run (y/n) OR your wiil want to regular tests (a)_");
            _createTest = Console.ReadLine();
            _doCreateTest();
            _browsers = new Driver(_testrail);
            FireFoxOnClick();
            ChromeOnClick();
            SafariOnClick();
            OperaOnClick();
            //EdgeOnClick();
        }
        private void _doAddSuite()
        {
            if (_addSuite == "y")
            {
                _testrail.AddCases();
            }
                
            else
                Console.WriteLine("You choose NO");
        }
        private void _doCreateTest()
        {
            if (_createTest == "a")
            {
                DateTime date = DateTime.Today;
                string nameSuite = date.DayOfWeek + " " + _getDateForJasonRequest(date);
                Console.WriteLine("\nThe test name is: " + nameSuite);
                _testrail.CreateRun(_testrail.GetSuiteID = date.DayOfWeek.ToString(), nameSuite);
            }
            if (_createTest == "y")
            {
                Console.WriteLine("\nPlease, enter name of run-test & SuitesID:");
                Console.Write("Name _");
                string nameSuite = Console.ReadLine();
                Console.Write("SuitesID _");
                string suiteID = Console.ReadLine();
                _testrail.CreateRun(_testrail.GetSuiteID = suiteID, nameSuite);
            }
            if (_createTest == "n")
            {
                Console.Write("Input run ID _");
                _testrail.RunID = Console.ReadLine();
                /* suite id назначается автоматически 
                 * из уже имеющегося, ранее созданного test run                 
                 * текст для свойства GetSuiteID задается опционально
                 */
                _testrail.GetSuiteID = "#$%";
                Console.WriteLine("Test is running..." + DateTime.Now);
            }
            else if (_createTest != "a" && _createTest != "y" && _createTest != "n")
            {
                Console.Write("InCorrect command...\nDo you want create a test-run (y/n) OR your wiil want to regular tests (a)_");
                _createTest = Console.ReadLine();
                _doCreateTest();
            }
        }
        private void DoYoyWantDeleteTest()
        {
            while (_deleteTest == "y")
            {
                Console.Write("Please, input run ID: ");
                _testrail.DeleteRun(Console.ReadLine());
                _testrail.GetSuitesOfProject();
                _testrail.GetRunsProject();
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
        private void EdgeOnClick()
		{

            RunBrowser(new EdgeDriver());			
		}
        private void SafariOnClick()
		{
            RunBrowser(new SafariDriver());
		}
        private void RunBrowser(IWebDriver webDriver)
        {
            //Browsers.Drivers.Add(webDriver);           
            _browsers.NavigateDriver(webDriver);
            webDriver.Quit();
        }
        private string _getDateForJasonRequest(DateTime date)
        {
            string month = null;
            if (date.Day < 10)
                month = "0" + date.Day + ".";
            else
                month = date.Day + ".";

            if (date.Month < 10)
                month += "0" + date.Month + "." + date.Year;
            else
                month += date.Month + "." + date.Year;
            return month;
        } 
    }
     
}
