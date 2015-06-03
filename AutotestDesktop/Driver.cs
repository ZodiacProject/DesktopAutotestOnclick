﻿using System;
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
        private bool _isLandChecked;
        private bool _isOnClick;
        private int _countWindowClick = 0;
        public List<IWebDriver> Drivers {get; private set;}
        public TestRail TestRun { get; set; }
//Constuctor
        public Driver()
        {
			Drivers = new List<IWebDriver>();
           _driverSettings = new List<PublisherTarget>()
                {
                new PublisherTarget() { Url = "http://putlocker.is", ZoneId = "10802", CountShowPopup = 3, IntervalPopup = 10000, StepCase = 0},
                new PublisherTarget() { Url = "http://thevideos.tv/", ZoneId = "90446", CountShowPopup = 3, IntervalPopup = 45000, TargetClick = "morevids", StepCase = 1},              
                new PublisherTarget() { Url = "http://www13.zippyshare.com/v/94311818/file.html/", ZoneId = "180376", CountShowPopup = 2, IntervalPopup = 45000, StepCase = 2},
                new PublisherTarget() { Url = "http://um-fabolous.blogspot.ru/", ZoneId = "199287", CountShowPopup = 3, IntervalPopup = 45000, StepCase = 3},                
                //new PublisherTarget() {Url = "http://www.flashx.tv/&?", ZoneId = "119133", CountShowPopup = 1, IntervalPopup = 20000, StepCase = 4},              
                };
        }
        //methods
public void NavigateDriver(IWebDriver driver)
        {
            TestRail TestRun = new TestRail();
            TestRun.StartTestRail();
            List<string> CaseToRun = new List<string>();
            
            foreach (string runCase in TestRun.GetRunCase(driver))
                     CaseToRun.Add(runCase);

            //foreach (string c in CaseToRun)
            //    Console.WriteLine(c);
            //return;
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
                          OnclickProgress(driver, driverSet);
                             if (!_isOnClick)
                             {
                                 errorMessage = "Во время клика не отработал показ. На сайте присутствует наш Network";
                                 commentMessage = "OnClick не отработал";
                                 Console.Error.WriteLine(driver.SwitchTo().Window(baseWindow).Url + " OnClick is " + _isOnClick);
                                 TestRun.SetStatus(CaseToRun[driverSet.StepCase], 5, errorMessage, commentMessage);
                                 break;
                             }
                      }
                      else
                      {
                           errorMessage = "FailedLand: " + failedLand + "\nLanding is " + _isLandChecked;
                           commentMessage = "Landing is " + _isLandChecked;
                           Console.Error.WriteLine(driver.SwitchTo().Window(baseWindow).Url + errorMessage);
                           TestRun.SetStatus(CaseToRun[driverSet.StepCase], 5, errorMessage, commentMessage);
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
                    TestRun.SetStatus(CaseToRun[driverSet.StepCase], 1, successMessage, null);
                }

                else if (_isLandChecked && _isOnClick)
                {
                    retestMessage = "Landing is " + _isLandChecked + " "
                        + driver.Url + " OnClick: popups is " + driverSet.CountShowPopup +
                        " & count of windows " + _countWindowClick + "\nIn the testing process is NOT open our Landing" + 
                        "\nPlease, repeat this test";
                    Console.Error.WriteLine(errorMessage + " " + _isOnClick);

                    TestRun.SetStatus(CaseToRun[driverSet.StepCase], 4, retestMessage, null);
                }
                   
            }//end foreach
        }
public void OnclickProgress (IWebDriver driver, PublisherTarget d_setting)
        {
                if ((_countWindowClick = driver.WindowHandles.Count) > 1)
                {
                    _isOnClick = true;
                    Thread.Sleep(2000);
                    try
                    {
                        foreach (string handle in driver.WindowHandles)
                        {
                            if (handle != driver.WindowHandles.ElementAt(0))//(driver.SwitchTo().Window(handle).Url != driver.SwitchTo().Window(baseWindow).Url)
                            {
                                driver.SwitchTo().Window(handle).Close();
                                while ((_countWindowClick = driver.WindowHandles.Count) > 1)
                                    {
                                        driver.SwitchTo().Alert().Accept(); // если появился alert      
                                    }    
                            }
                        }
                    }
                    catch (Exception e) { Console.WriteLine(e); }
                    //if ((_countWindowClick = driver.WindowHandles.Count) > 1)
                    //    driver.Close();
                }

                d_setting.CountShowPopup--;
                driver.SwitchTo().Window(driver.WindowHandles.ElementAt(0));

                //    if (driver.Url != "http://thevideos.tv/")
                //      driver.Navigate().Back();

                // time Interval popup
                Thread.Sleep(d_setting.IntervalPopup); 
        }
    }
}
    

