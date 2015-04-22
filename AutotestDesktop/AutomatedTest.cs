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
	[TestClass]
	public class AutomatedTest
	{
		Driver Browser = new Driver();
        private void RunBrowser(IWebDriver webDriver)
        {
           // Task.Factory.StartNew(() =>
            //{

                Browser.Drivers.Add(webDriver);
                Browser.NavigateDriver(webDriver);
                webDriver.Quit();
                
           // });
        }
        [TestMethod]
		public void FireFoxOnClick()
		{   
			RunBrowser(new FirefoxDriver());
		}
       //[TestMethod]
		public void ChromeOnClick()
		{
              
            RunBrowser(new ChromeDriver());
		}
       // [TestMethod]
		public void OperaOnClick()
		{
            RunBrowser(new OperaDriver());
		
		}
        // [TestMethod]
		public void IEOnClick()
		{
            RunBrowser(new InternetExplorerDriver());
			
		}
       // [TestMethod]
		public void SafariOnClick()
		{
            RunBrowser(new SafariDriver());
			
		}

		[TestInitialize]
		public void Setup()
		{

		}
		// Close Browser
		//  [TestCleanup]

	}
     
}
