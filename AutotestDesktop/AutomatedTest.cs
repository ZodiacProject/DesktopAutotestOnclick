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
using OpenQA.Selenium.PhantomJS;
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
        public AutomatedTest(string runID)
        {
            _testrail = new TestRail();
/* suite id назначается автоматически 
*  из уже имеющегося, ранее созданного test run                 
*  текст для свойства GetSuiteID задается опционально
*/
            _testrail.GetRunsProject();
            _testrail.RunID = runID;
            _testrail.GetSuiteID = "#$%";
            Console.WriteLine("Test is running..." + DateTime.Now);
            _browsers = new Driver(_testrail);
            FireFoxOnClick();
            //PhantomOnClick();
        }    
		private void FireFoxOnClick()
		{   
			RunBrowser(new FirefoxDriver());
		}
		private void PhantomOnClick()
		{             
            RunBrowser(new PhantomJSDriver());
		}
      

        private void RunBrowser(IWebDriver webDriver)
        {                
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
