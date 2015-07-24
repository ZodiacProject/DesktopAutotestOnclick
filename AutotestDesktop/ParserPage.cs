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
        private bool _isFindZone = false;
        private IReadOnlyCollection<IWebElement> _searchWebelements;
        private List<string> _searchElement = new List<string>();
        public bool FindZoneOnPage(IWebDriver driver, string url, string zoneID)
        {            
            driver.Navigate().GoToUrl(url);
            _searchWebelements = driver.FindElements(By.TagName("a"));
            foreach (IWebElement webEl in _searchWebelements)
            {
                if (webEl.Text != string.Empty)
                    _searchElement.Add(webEl.Text);
            }
            foreach (string  element in _searchElement)
            {             
                try
                {
                    foreach (string zone in _concatZoneID(zoneID))
                    {
                        if (driver.PageSource.Contains(zone))
                        {
                            Console.WriteLine("Yes, I'am find zone");
                            return _isFindZone = true;
                        }
                    }
                    if (!_isFindZone)
                    {
                        driver.FindElement(By.LinkText(element)).Click();
                        Thread.Sleep(3000);
                    }
                    driver.Navigate().Back();
                }
                catch { }
            }
            return false;            
        }
        private List <string> _concatZoneID (string str)
        {
            List <string> LconcatZoneId = new List<string>();
            string concatZone = null;
            for (int i = 0; i < str.Length; i++)
            {
                if (str[i] != '#')
                    concatZone += str[i];
            }
            LconcatZoneId.Add(concatZone);
            return LconcatZoneId;
        }
    }
    
}
