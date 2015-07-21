using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Opera;
using OpenQA.Selenium.Safari;
using OpenQA.Selenium.Support;
using OpenQA.Selenium.Internal;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AutotestDesktop
{
    class ParserPage
    {
        private IWebDriver _driver;
        private IReadOnlyCollection<IWebElement> _searchWebelements;
        private List<string> _searchElement = new List<string>();
        public ParserPage(IWebDriver driver)
        {
            _driver = driver;
            _driver.Navigate().GoToUrl("http://thevideos.tv");
            _searchWebelements = _driver.FindElements(By.TagName("a"));
            foreach (IWebElement webEl in _searchWebelements)
            {
                if (webEl.Text != string.Empty)
                    _searchElement.Add(webEl.Text);
            }
            foreach (string  element in _searchElement)
            {             
                try
                {
                    _driver.FindElement(By.LinkText(element)).Click();
                    Thread.Sleep(3000);             
                    if (driver.PageSource.Contains("90446"))
                    {
                        Console.WriteLine("Yes, I'am find zone");
                        return;
                    }
                    _driver.Navigate().Back();
                }
                catch { }
            }
            
        }
    }
    
}
