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
        private bool _isLandChecked;
        private int _countWindowClick = 0;
        public List<IWebDriver> Drivers {get; private set;}
        public TestRail TestRun { get; set; }
//Constuctor
        public Driver()
        {
			Drivers = new List<IWebDriver>();
           _driverSettings = new List<PublisherTarget>()
                {
              // new PublisherTarget() { Url = "http://putlocker.is", ZoneId = "10802", CountShowPopup = 3, IntervalPopup = 10000, StepCase = 0},
              // new PublisherTarget() { Url = "http://thevideos.tv/", ZoneId = "90446", CountShowPopup = 4, IntervalPopup = 30000, TargetClick = "morevids", StepCase = 1},            
                //new PublisherTarget() { Url = "http://vodlocker.com/", ZoneId = "61593", ShowPopup = ??, IntervalPopup = ??, StepCase = 2 },
                //new PublisherTarget() { Url = "http://www13.zippyshare.com/v/94311818/file.html/", ZoneId = "180376", CountShowPopup = 2, IntervalPopup = 45000, StepCase = 3},
               // new PublisherTarget() { Url = "http://um-fabolous.blogspot.ru/", ZoneId = "199287", CountShowPopup = 3, IntervalPopup = 45000, StepCase = 4},                
               // new PublisherTarget() {Url = "http://www.flashx.tv/&?", ZoneId = "119133", CountShowPopup = 1, IntervalPopup = 20000, StepCase = 5},              
                };
        }
        //methods
        public void NavigateDriver(IWebDriver driver)
        {
            TestRail TestRun = new TestRail();
            List<string> CaseToRun = new List<string>();
            
            foreach (string runCase in TestRun.GetRunCase(driver))
                     CaseToRun.Add(runCase);

            //foreach (string c in CaseToRun)
            //   Console.WriteLine(c);
            // return;
            string successMessage = "";
            string errorMessage = "";
            string retestMessage = "";
            foreach (PublisherTarget driverSet in _driverSettings)
            {
                driver.Navigate().GoToUrl(driverSet.Url);
            
                
                int failedLand = 0;
                // Проверка на наш Landing
                _isLandChecked = driver.PageSource.Contains(driverSet.ZoneId); //false;//
              
                //string checkLandStr = "http://onclickads.net/afu.php?id=";
                string baseWindow = driver.CurrentWindowHandle;
                //return;
                if (driver.Url == "http://thevideos.tv/")
                {
                    driver.FindElement(By.ClassName(driverSet.TargetClick)).Click();
                    _isLandChecked = driver.PageSource.Contains(driverSet.ZoneId);
                }
                while (driverSet.CountShowPopup != 0)
                {
                       Thread.Sleep(3000);
                       driver.SwitchTo().ActiveElement().Click();
                                
                       if (_isLandChecked)
                       {
                           if ((_countWindowClick = driver.WindowHandles.Count) > 1)
                           {
                               try
                               {
                                   foreach (string handle in driver.WindowHandles)
                                   {
                                       if (driver.SwitchTo().Window(handle).Url != driver.SwitchTo().Window(baseWindow).Url)
                                       {
                                           driver.SwitchTo().Window(handle);
                                           driver.Close();//закрытие всплывающего окна
                                       }
                                   }
                               }
                               catch (Exception) { }
                               try
                               {
                                   do
                                   {
                                       driver.SwitchTo().Alert().Accept(); // если появился alert      
                                   } while ((_countWindowClick = driver.WindowHandles.Count) > 1);
                               }
                               catch (Exception) { }
                           }
                           
                           driverSet.CountShowPopup--;
                           driver.SwitchTo().Window(baseWindow);

                           //    if (driver.Url != "http://thevideos.tv/")
                           //      driver.Navigate().Back();

                           // time Interval popup
                           Thread.Sleep(driverSet.IntervalPopup);
                       }
                       else
                           failedLand++;
                      if (failedLand > 3)
                       {
                           errorMessage = driver.SwitchTo().Window(baseWindow).Title + " FailedLand: " + failedLand + "\nLanding is " + _isLandChecked;
                           Console.Error.WriteLine(driver.SwitchTo().Window(baseWindow).Title + errorMessage);
                           TestRun.SetStatus(CaseToRun[driverSet.StepCase], 5, errorMessage);
                           break;
                       }
                     //*
                   //  isLandChecked = false;
                    //*
                }
    // Проверка на открытие после того, как все показы уже были
                try
                { 
                    driver.SwitchTo().ActiveElement().Click(); 
                }
                catch (Exception) { }

                _countWindowClick = driver.WindowHandles.Count;
                if (_countWindowClick == 1 && driverSet.CountShowPopup == 0)
                { 
                    Console.WriteLine(successMessage + _isLandChecked);
                    successMessage = driver.Url + "\nLanding is - " + _isLandChecked;
                    TestRun.SetStatus(CaseToRun[driverSet.StepCase], 1, successMessage);
                }

                else if (_isLandChecked)
                {
                    retestMessage = "Landing is " + _isLandChecked + " "
                        + driver.Url + " OnClick: popups is " + driverSet.CountShowPopup +
                        " & count of windows " + _countWindowClick + " repeat a test";
                    Console.Error.WriteLine(errorMessage);

                    TestRun.SetStatus(CaseToRun[driverSet.StepCase], 4, retestMessage);
                }
                   
            }//end foreach
        }
        public void OnclickProgress (IWebDriver driver, PublisherTarget d_setting, string baseWindow)
        {
            
            if (_isLandChecked)
            {
                if ((_countWindowClick = driver.WindowHandles.Count) > 1)
                {
                    try
                    {
                        foreach (string handle in driver.WindowHandles)
                        {
                            if (driver.SwitchTo().Window(handle).Url != driver.SwitchTo().Window(baseWindow).Url)
                            {
                                driver.SwitchTo().Window(handle);
                                driver.Close();//закрытие всплывающего окна
                            }
                        }
                    }
                    catch (Exception) { }
                    try
                    {
                        do
                        {
                            driver.SwitchTo().Alert().Accept(); // если появился alert      
                        } while ((_countWindowClick = driver.WindowHandles.Count) > 1);
                    }
                    catch (Exception) { }
                }

                d_setting.CountShowPopup--;
                driver.SwitchTo().Window(baseWindow);

                //    if (driver.Url != "http://thevideos.tv/")
                //      driver.Navigate().Back();

                // time Interval popup
                Thread.Sleep(d_setting.IntervalPopup);
            }
        }
    }
}
    

