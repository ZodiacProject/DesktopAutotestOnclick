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
        private List<string> CaseToRun = new List<string>();
        private List<string> TopSitesOnClick = new List<string>();
        private Dictionary<string, string> TestCase = new Dictionary<string, string>();
        private bool _isLandChecked;
        private bool _isOnClick;
        private bool _isLoadPage;
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
            foreach (string runCase in _testRun.GetRunCase(driver))
                     CaseToRun.Add(runCase);
            foreach (KeyValuePair <string, string> testcase in _testRun.TestCaseName)
                     TestCase.Add(testcase.Key, testcase.Value);
 
            //foreach (string topSite in _testRun.TopOnclick)
              //  Console.WriteLine(topSite);

            //foreach (string c in CaseToRun)
            //    Console.WriteLine(c);
            //return;

            _publishers = new PublisherTarget();
            _driverSettings = _publishers.GetDriverSettings(TestCase);
            ParserPage parsePage = new ParserPage();
            string baseWindow = null;
            string successMessage = "";
            string errorMessage = "";
            string retestMessage = "";
            string commentMessage = "";
            foreach (PublisherTarget driverSet in _driverSettings)
            {                
                driver.Navigate().GoToUrl(driverSet.Url);
                baseWindow = driver.CurrentWindowHandle;
                while ((driver.WindowHandles.Count) > 1)
                {
                    _closeOtherWindows(driver, baseWindow);
                    driver.SwitchTo().Window(driver.WindowHandles.ElementAt(0));
                }     
                if (driver.Title.Contains("недоступен"))
                {
                    Console.WriteLine(driver.Url + " Веб-страница недоступна");
                    _isLoadPage = false;
                }
                else
                    _isLoadPage = true;

                if (parsePage.FindZoneOnPage(driver, driverSet.Url, driverSet.ZoneIds))
                    _isLandChecked = true;
                else
                    _isLandChecked = false;                                        
                while (driverSet.CountShowPopup != 0)
                {                 
                    driver.SwitchTo().Window(driver.WindowHandles.ElementAt(0)).SwitchTo().ActiveElement().Click();
                    Thread.Sleep(3000);
                      if (_isLandChecked)
                      {
                          _onclickProgress(driver, driverSet);
                             if (!_isOnClick)
                             {
                                 errorMessage = "Во время клика не отработал показ. На сайте присутствует наш Network";
                                 commentMessage = "OnClick не отработал";
                                 Console.Error.WriteLine(driver.SwitchTo().Window(baseWindow).Url + " OnClick is " + _isOnClick);
                                 _testRun.SetStatus(CaseToRun[driverSet.StepCase], _testRun.Status = Status.Failed, errorMessage, commentMessage);
                                 break;
                             }
                      }
                      else
                      {
                          if (_isLoadPage)
                              errorMessage = " Landing is " + _isLandChecked;
                          else
                              errorMessage = " Веб-страница недоступна";
                           commentMessage = errorMessage;
                           Console.Error.WriteLine(driver.SwitchTo().Window(baseWindow).Url + errorMessage);
                           _testRun.SetStatus(CaseToRun[driverSet.StepCase], _testRun.Status = Status.Failed, errorMessage, commentMessage);
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
                if (_countWindowClick == 1 && driverSet.CountShowPopup == 0 && _isLandChecked)
                {                  
                    successMessage = driver.Url + "\nLanding is - " + _isLandChecked;
                    Console.WriteLine(successMessage + " " + _isLandChecked + " " + _isOnClick);
                    _testRun.SetStatus(CaseToRun[driverSet.StepCase], _testRun.Status = Status.Passed, successMessage, null);
                }

               else if (_isLandChecked && _isOnClick)
                {
                    retestMessage = "Landing is " + _isLandChecked + " "
                        + driver.Url + " OnClick: popups is " + driverSet.CountShowPopup +
                        " & count of windows " + _countWindowClick + "\nIn the testing process is NOT open our Landing" + 
                        "\nPlease, repeat this test";
                    Console.Error.WriteLine(errorMessage + " " + _isOnClick);

                    _testRun.SetStatus(CaseToRun[driverSet.StepCase], _testRun.Status = Status.Retest, retestMessage, null);
                }
                //else if (!_isLandChecked && _isLoadPage)
                //{
                //    errorMessage = "Страница не содержит наш тег";
                //    commentMessage = "Landing is " + _isLandChecked;
                //    Console.Error.WriteLine(driver.SwitchTo().Window(baseWindow).Url + errorMessage);
                //    _testRun.SetStatus(CaseToRun[driverSet.StepCase], _testRun.Status = Status.Failed, errorMessage, commentMessage);            
                //}                   
            }//end foreach
        }

private void _closeOtherWindows(IWebDriver driver, string basicWin)
{
    IReadOnlyCollection<string> windows = driver.WindowHandles;
    foreach (string win in windows)
        try
        {
            if (win != basicWin)
            {
                driver.SwitchTo().Window(win).Close();
                if ((driver.WindowHandles.Count) > 1)
                    _acceptAlert(driver);
            }
        }
        catch { }
}
private void _onclickProgress(IWebDriver driver, PublisherTarget d_setting)
{
    try
    {
        if ((_countWindowClick = driver.WindowHandles.Count) > 1)
        {
            _isOnClick = true;
            driver.SwitchTo().Window(driver.WindowHandles.ElementAt(1)).Close();
            Thread.Sleep(2000);
            if ((_countWindowClick = driver.WindowHandles.Count) > 1)
            {
                _acceptAlert(driver);
                if ((_countWindowClick = driver.WindowHandles.Count) > 1)
                    _acceptAlert(driver);
            }
        }
        else
            _isOnClick = false;
    }
    catch { }
        driver.SwitchTo().Window(driver.WindowHandles.ElementAt(0));
        d_setting.CountShowPopup--;
        Thread.Sleep(d_setting.Interval);
}
private void _acceptAlert(IWebDriver driver)
{
    string alertText = "";
    IAlert alert = null;
    int count = 0;
    while (alertText.Equals("") && count < 2)
    {
        if (alert == null)
        {
            try
            {
                alert = driver.SwitchTo().Alert();
            }
            catch { Thread.Sleep(50); }
        }
        else
        {
            try
            {
                alert.Accept();
                alertText = alert.Text;
            }
            catch (Exception ex)
            {
                if (ex.Message.Equals("No alert is present"))
                    alertText = "Already Accepted";
                else
                    Thread.Sleep(50);
            }
        }
        count++;
    }
}
    }
}
    

