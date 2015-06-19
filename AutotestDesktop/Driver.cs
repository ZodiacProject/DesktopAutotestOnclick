using System;
using Newtonsoft.Json.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Opera;
using OpenQA.Selenium.Safari;
using OpenQA.Selenium.Support;
using OpenQA.Selenium.Internal;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using NuLog;


namespace AutotestDesktop
{
    class Driver
    {
        private List<PublisherTarget> _driverSettings; // for many publishers
        private PublisherTarget _publishers;
        private bool _isLandChecked;
        private bool _isOnClick;
        private int _countWindowClick = 0;
        public List<IWebDriver> Drivers {get; private set;}
        private TestRail _testRun;
//Constuctor
        public Driver(TestRail test)
        {
            _testRun = test;
			Drivers = new List<IWebDriver>();
        }
        //methods
public void NavigateDriver(IWebDriver driver)
        {
            _testRun.StartTestRail();
            List<string> CaseToRun = new List<string>();
            List<string> NameTestCase = new List<string>();
            List<string> TopSitesOnClick = new List<string>();

            foreach (string runCase in _testRun.GetRunCase(driver))
                     CaseToRun.Add(runCase);
            foreach (string testName in _testRun.TestCaseName)
                     NameTestCase.Add(testName);
          //foreach (string topSite in _testRun.TopOnclick)
            //    Console.WriteLine(topSite);
           //_testRun.UpdateSuite("117");
            return;

            //foreach (string c in CaseToRun)
            //    Console.WriteLine(c);
            //return;

            _publishers = new PublisherTarget();
            _driverSettings = _publishers.GetDriverSettings(NameTestCase);

            string successMessage = "";
            string errorMessage = "";
            string retestMessage = "";
            string commentMessage = "";
            foreach (PublisherTarget driverSet in _driverSettings)
            {
                driver.Navigate().GoToUrl(driverSet.Url);
        
                int failedLand = 0;
                // Проверка на наш Landing
                if (driver.Url != "http://thevideos.tv/")
                {
                  //  _FindZoneID(driver);
                  //  return;
                    if (driver.PageSource.Contains(driverSet.ZoneId))
                        _isLandChecked = true;
                    else
                        _isLandChecked = false;
                }
                else
                {
                     driver.FindElement(By.ClassName(driverSet.TargetClick)).Click();
                     Thread.Sleep(3000);
                        if (driver.PageSource.Contains(driverSet.ZoneId))
                            _isLandChecked = true;
                        else
                            _isLandChecked = false;
                }

                string baseWindow = driver.CurrentWindowHandle;
              
                while (driverSet.CountShowPopup != 0)
                {
                    if (driver.Url != "http://thevideos.tv/")
                            Thread.Sleep(2000);

                    driver.SwitchTo().Window(driver.WindowHandles.ElementAt(0)).SwitchTo().ActiveElement().Click();
                    Thread.Sleep(3000);
                      if (_isLandChecked)
                      {
                          _OnclickProgress(driver, driverSet);
                             if (!_isOnClick)
                             {
                                 errorMessage = "Во время клика не отработал показ. На сайте присутствует наш Network";
                                 commentMessage = "OnClick не отработал";
                                 Console.Error.WriteLine(driver.SwitchTo().Window(baseWindow).Url + " OnClick is " + _isOnClick);
                                 _testRun.SetStatus(CaseToRun[driverSet.StepCase], 5, errorMessage, commentMessage);
                                 break;
                             }
                      }
                      else
                      {
                           errorMessage = "FailedLand: " + failedLand + "\nLanding is " + _isLandChecked;
                           commentMessage = "Landing is " + _isLandChecked;
                           Console.Error.WriteLine(driver.SwitchTo().Window(baseWindow).Url + errorMessage);
                           _testRun.SetStatus(CaseToRun[driverSet.StepCase], 5, errorMessage, commentMessage);
                           break;
                       }

                }
    // Проверка на открытие после того, как все показы уже были
                try
                {
                    driver.SwitchTo().Window(driver.WindowHandles.ElementAt(0)).SwitchTo().ActiveElement().Click();
                }
                catch { }

                _countWindowClick = driver.WindowHandles.Count;
                if (_countWindowClick == 1 && driverSet.CountShowPopup == 0)
                {
                    successMessage = driver.Url + "\nLanding is - " + _isLandChecked;
                    Console.WriteLine(successMessage + " " + _isLandChecked + " " + _isOnClick);
                    _testRun.SetStatus(CaseToRun[driverSet.StepCase], 1, successMessage, null);
                }

                else if (_isLandChecked && _isOnClick)
                {
                    retestMessage = "Landing is " + _isLandChecked + " "
                        + driver.Url + " OnClick: popups is " + driverSet.CountShowPopup +
                        " & count of windows " + _countWindowClick + "\nIn the testing process is NOT open our Landing" + 
                        "\nPlease, repeat this test";
                    Console.Error.WriteLine(errorMessage + " " + _isOnClick);

                    _testRun.SetStatus(CaseToRun[driverSet.StepCase], 4, retestMessage, null);
                }
                   
            }//end foreach
        }
private string _FindZoneID(IWebDriver driver)
{
    if (driver.PageSource.Contains("apu.php?zoneid="))
    {
        //IList<IWebElement> Script = driver.FindElements(By.XPath("//script"));
        //foreach (IWebElement script in Script)
       IWebElement el = driver.FindElement(By.XPath("/html/body/script[1]"));
            Console.WriteLine(el.Text);
    }
    return null;
}
private void _OnclickProgress (IWebDriver driver, PublisherTarget d_setting)
        {
                if ((_countWindowClick = driver.WindowHandles.Count) > 1)
                {
                    _isOnClick = true;                    
                    try
                    {
                        driver.SwitchTo().Window(driver.WindowHandles.ElementAt(1)).Close();
                        while ((_countWindowClick = driver.WindowHandles.Count) > 1)
                        {
                            driver.SwitchTo().Alert().Accept(); // если появился alert      
                        }    
                    }
                    catch (Exception e) { Console.WriteLine(e); }
                }

                d_setting.CountShowPopup--;
                driver.SwitchTo().Window(driver.WindowHandles.ElementAt(0));

                //    if (driver.Url != "http://thevideos.tv/")
                //      driver.Navigate().Back();

                // time Interval popup
                Thread.Sleep(d_setting.Interval); 
        }
    }
}
    

