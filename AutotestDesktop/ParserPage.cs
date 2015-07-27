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
        private IReadOnlyCollection <IWebElement> _searchWebelements;
        private List <string> _searchElement = new List<string>();
        public bool FindZoneOnPage(IWebDriver driver, string url, string zoneID)
        {
            driver.Navigate().GoToUrl(url);
            if (_сheckZone(driver, zoneID))
            {
                _isFindZone = true;
                return _isFindZone;
            }
            else
            {
                _searchWebelements = driver.FindElements(By.TagName("a"));
                foreach (IWebElement webEl in _searchWebelements)
                {
                    if (webEl.Text != string.Empty)
                        _searchElement.Add(webEl.Text);
                }
                foreach (string element in _searchElement)
                {
                    try
                    {
                        driver.FindElement(By.LinkText(element)).Click();
                        Thread.Sleep(3000);
                        if (_сheckZone(driver, zoneID))
                        {
                            _isFindZone = true;
                            return _isFindZone;
                        }
                        else
                            driver.Navigate().Back();
                    }
                    catch { }
                }
                return false;
            }
        }

private bool _сheckZone(IWebDriver driver, string zoneID)
    {
        foreach (string zone in _concatZoneID(zoneID))
        {
            if (driver.PageSource.Contains(zone))
            {
                Console.WriteLine("Yes, I'am find zone");
                return true;
            }
        }
        return false;
    }
private List <string> _concatZoneID (string str)
    {
        List <string> LconcatZoneId = new List<string>();
        string concatZone = null;
        if (str.Contains("#"))
            for (int i = 0; i < str.Length; i++)
            {
                if (str[i] != '#')
                    concatZone += str[i];
                else
                    LconcatZoneId.Add(concatZone);
            }
        else
            LconcatZoneId.Add(str);
        return LconcatZoneId;
    }
    }
    
}
