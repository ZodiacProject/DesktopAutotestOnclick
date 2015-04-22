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
using System;
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

        public List<IWebDriver> Drivers {get; private set;}
        private List<PublisherTarget> _driverSettings; // for many publishers
//Constuctor
        public Driver()
        {
			Drivers = new List<IWebDriver>();
           _driverSettings = new List<PublisherTarget>()
                {
              //  new PublisherTarget() { Url = "http://putlocker.is", ZoneId = "10802", ShowPopup = 3, IntervalPopup = 10000 },
                new PublisherTarget() {Url = "http://www.flashx.tv/&?", ZoneId = "119133", ShowPopup = 1, IntervalPopup = 20000},
              //  new PublisherTarget() { Url = "http://thevideos.tv/", ZoneId = "90446", ShowPopup = 4, IntervalPopup = 30000, TargetClick = "morevids" },
              //  new PublisherTarget() { Url = "http://www13.zippyshare.com/v/94311818/file.html/", ZoneId = "180376", ShowPopup = 2, IntervalPopup = 45000 },
                new PublisherTarget() { Url = "http://um-fabolous.blogspot.ru/", ZoneId = "199287", ShowPopup = 3, IntervalPopup = 45000 },
                //new PublisherTarget() { Url = "http://vodlocker.com/", ZoneId = "61593", ShowPopup = ??, IntervalPopup = ?? },
                };
        }
        //methods
        public void NavigateDriver(IWebDriver driver)
        {
            foreach (PublisherTarget driverSet in _driverSettings)
            {
               driver.Navigate().GoToUrl(driverSet.Url);
                int countWindowClick = 0;
                int failedLand = 0;
                // Проверка на наш Landing
                bool isLandChecked = driver.PageSource.Contains(driverSet.ZoneId); //false;//
                //string checkLandStr = "http://onclickads.net/afu.php?id=";
                string baseWindow = driver.CurrentWindowHandle;
                //return;
                if (driver.Url == "http://thevideos.tv/")
                    driver.FindElement(By.ClassName(driverSet.TargetClick)).Click();
                while (driverSet.ShowPopup != 0)
                {
                   // if (driver.Url == "http://thevideos.tv/")
                     //   driver.FindElement(By.ClassName(driverSet.TargetClick)).Click();
                       Thread.Sleep(3000);
                       driver.SwitchTo().ActiveElement().Click();             
                      
                       if (isLandChecked)
                       {
                           if ((countWindowClick = driver.WindowHandles.Count) > 1)
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
                                   } while ((countWindowClick = driver.WindowHandles.Count) > 1);
                               }
                               catch (Exception) { }
                           }
                           
                           driverSet.ShowPopup--;
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
                           Console.Error.WriteLine(driver.SwitchTo().Window(baseWindow).Title + " FailedLand: " + failedLand + " Repeat a test");
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

                countWindowClick = driver.WindowHandles.Count;
                if (countWindowClick == 1 && driverSet.ShowPopup == 0)
                    Console.WriteLine("[Passed] " + driver.Url + "\nLanding is - " + isLandChecked);
                else if (isLandChecked)
                    Console.Error.WriteLine("### [FAILED] " + "Landing is " + isLandChecked + " " + driver.SwitchTo().Window(baseWindow).Title + " OnClick: popups is " + driverSet.ShowPopup + " & count of windows " + countWindowClick);
            }//end foreach
        }
    }
}
    

