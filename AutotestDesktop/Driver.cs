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
        private TestRail _testRun;
      
        public List<IWebDriver> Drivers {get; private set;}
//Constuctor
        public Driver(TestRail test)
        {
            _testRun = test;
			Drivers = new List<IWebDriver>();
        }
        //methods
public void NavigateDriver(IWebDriver driver)
        {
            string successMessage = "";
            string errorMessage = "";
            string retestMessage = "";
            string commentMessage = "";
            string baseWindow = "";

            _testRun.StartTestRail();
            _publishers = new PublisherTarget();
            List<string> CaseToRun = new List<string>();
            List<string> NameTestCase = new List<string>();  

            foreach (string runCase in _testRun.GetRunCase(driver))
                     CaseToRun.Add(runCase);
            foreach (string testName in _testRun.GetNameCase())
                     NameTestCase.Add(testName);
  
            _driverSettings = _publishers.GetDriverSettings(NameTestCase);

            foreach (PublisherTarget driverSet in _driverSettings)
            {
                driver.Navigate().GoToUrl(driverSet.Url);
                Thread.Sleep(2000);

                baseWindow = driver.CurrentWindowHandle;           
                // Проверка на наш Landing
                if (driver.Url != "http://thevideos.tv/")
                {
                    if (driver.PageSource.Contains(driverSet.ZoneId))
                        _isLandChecked = true;
                    else
                        _isLandChecked = false; 
                }
                else
                {
                    driver.FindElement(By.ClassName(driverSet.TargetClick)).Click();
                    Thread.Sleep(2000);
                    driverSet.Url = driver.Url;
                        if (driver.PageSource.Contains(driverSet.ZoneId))
                            _isLandChecked = true;
                        else
                            _isLandChecked = false;
                }
                if (_isLandChecked)                                         
                    while (driverSet.CountShowPopup != 0)
                    {
                        Thread.Sleep(1000);

                        try
                        {
                            if (driver.Url != driverSet.Url)
                            {
                                driver.Navigate().GoToUrl(driverSet.Url);
                                Thread.Sleep(2000);
                            }                                
                            driver.SwitchTo().ActiveElement().Click();
                            Thread.Sleep(2000);
                            OnclickProgress(driver, driverSet);
                            if (!_isOnClick)
                                break;
                        }
                        catch { }
                    }
                
                else
                {
                    errorMessage = "Landing is " + _isLandChecked + "\nНа странице отсутсвует наш тег";
                    commentMessage = "Landing is " + _isLandChecked;
                    Console.Error.WriteLine(driver.Url + errorMessage);
                    _testRun.SetStatus(CaseToRun[driverSet.StepCase], 5, errorMessage, commentMessage);
                }                

    // Проверка на открытие после того, как все показы уже были
                if (_isOnClick && _isLandChecked)
                try
                {
                    driver.SwitchTo().Window(driver.WindowHandles.ElementAt(0)).SwitchTo().ActiveElement().Click();
                }
                catch { }

                if (!_isOnClick && _isLandChecked)
                {
                    errorMessage = "Во время клика не отработал показ. На сайте присутствует наш Network";
                    commentMessage = "OnClick не отработал. Тег есть на странице";
                    Console.Error.WriteLine(driver.Url + " OnClick is " + _isOnClick);
                    _testRun.SetStatus(CaseToRun[driverSet.StepCase], 5, errorMessage, commentMessage);
                }
                if ((_countWindowClick = driver.WindowHandles.Count) == 1 && driverSet.CountShowPopup == 0)
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

public void OnclickProgress (IWebDriver driver, PublisherTarget d_setting)
        {
            try
            {
                if ((_countWindowClick = driver.WindowHandles.Count) > 1)
                {
                    _isOnClick = true;                 
                    driver.SwitchTo().Window(driver.WindowHandles.ElementAt(1)).Close();
                    while ((_countWindowClick = driver.WindowHandles.Count) > 1)
                    {
                        driver.SwitchTo().Alert().Accept();
                        if ((_countWindowClick = driver.WindowHandles.Count) > 1)
                            driver.SwitchTo().Alert().Dismiss();
                        if ((_countWindowClick = driver.WindowHandles.Count) > 1)
                            driver.SwitchTo().Window(driver.WindowHandles.ElementAt(1)).Close();                       
                    }
                }
                else
                    _isOnClick = false;
                        
            driver.SwitchTo().Window(driver.WindowHandles.ElementAt(0));
            }
            catch { }
            d_setting.CountShowPopup--;
            Thread.Sleep(d_setting.Interval);            
            }
    }
}
    

