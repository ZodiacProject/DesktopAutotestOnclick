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
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Support;
using OpenQA.Selenium.Internal;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AutotestDesktop
{
    internal static class ParserPage
    {                
        private static IReadOnlyCollection <IWebElement> _searchWebelements;
        private static List <string> _searchElement = new List<string>();
        private static List <string> _zoneIdList;
        private static string _foundedUrlCheck = null;
        public static string GetFoundUrlCheck
        {
            get 
            { 
                return _foundedUrlCheck; 
            }
            set
            {
                _foundedUrlCheck = value;
            }
        }
        public static bool FindZoneOnPage(IWebDriver driver, string url, string zoneID)
        {
            _zoneIdList = zoneID.Split('#').ToList();
            if (_сheckZone(driver, _zoneIdList))
                return true;
            else if (_isfindoneOnLink(driver, url))
                return true;
            else 
                return false;
        }
private static bool _isfindoneOnLink(IWebDriver driver, string url)
        {
            _searchWebelements = driver.FindElements(By.TagName("a"));
                  
            foreach (IWebElement webEl in _searchWebelements)
            {
                if (webEl.Text != string.Empty)
                    _searchElement.Add(webEl.Text);
            }
            _cutSearchLinks();
            foreach (string element in _searchElement)
            {
                try
                {
                    driver.FindElement(By.LinkText(element)).Click();
                    Thread.Sleep(3000);
                    if (_сheckZone(driver, _zoneIdList))
                    {
                        GetFoundUrlCheck = driver.Url;
                        return true;
                    }
                    else if ((driver.WindowHandles.Count) > 1) // если ссылка открылась в другой вкладке/окне
                    {
                        driver.SwitchTo().Window(driver.WindowHandles.ElementAt(1)).Close(); 
                        driver.Navigate().GoToUrl(url);
                    }
                    else
                        driver.Navigate().GoToUrl(url);
                }
                catch { }
            }
            return false;
        }
private static void _cutSearchLinks ()
        {
            const int count_for_delete = 40;
                if (_searchElement.Count > count_for_delete)
                    _searchElement.RemoveRange(count_for_delete, _searchElement.Count - count_for_delete);                   
        }
public static bool IsZoneOnTestCase (string zoneID)
        {
            if (zoneID != "ZoneIsNull" && zoneID != "")
                return true;
            else
                return false;
        }
private static bool _сheckZone(IWebDriver driver, List <string> zoneID)
    {
        foreach (string zone in zoneID)
        {
            if (driver.PageSource.Contains(zone))
            {
                Console.WriteLine("Yes, I'am find zone " + zone + " " + driver.Url);
                return true;
            }
        }
        return false;
    }
    }
    
}
