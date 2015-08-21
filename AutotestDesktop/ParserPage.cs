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
    class ParserPage
    {
        private bool _isFindZone;
        private const int _elementsForRemove = 100; // когда размер ссылок на странице слишком велик
        private IReadOnlyCollection <IWebElement> _searchWebelements;
        private List <string> _searchElement = new List<string>();
        private List <string> _zoneIdList;
        public bool FindZoneOnPage(IWebDriver driver, string url, string zoneID)
        {
            _zoneIdList = zoneID.Split('#').ToList();
            if (_сheckZone(driver, _zoneIdList))
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
                _cutSearchLinks();
                foreach (string element in _searchElement)
                {
                    try
                    {
                        driver.FindElement(By.LinkText(element)).Click();
                        Thread.Sleep(3000);
                        if (_сheckZone(driver, _zoneIdList))
                        {
                            _isFindZone = true;
                            return _isFindZone;
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
        }
private void _cutSearchLinks ()
        {
            const int count_for_delete = 40;
                if (_searchElement.Count > count_for_delete)
                    _searchElement.RemoveRange(count_for_delete, _searchElement.Count - count_for_delete);                   
        }
public bool IsZoneOnTestCase (string zoneID)
        {
            if (zoneID != "ZoneIsNull" || zoneID != "")
                return true;
            else
                return false;
        }
private bool _сheckZone(IWebDriver driver, List <string> zoneID)
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
