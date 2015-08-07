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
        private PublisherTarget _publishers;
        private List<string> TopSitesOnClick = new List<string>();
        private Dictionary<string, List<string>> _sectionCaseToRun = new Dictionary<string, List<string>>();
        private Dictionary<string, string> _testCase = new Dictionary<string, string>();
        private bool _isLandChecked;
        private bool _isOnClick;
        private bool _isLoadPage;      
        private bool _isZoneOnTestCase;
        private TestRail _testRun;
//Constuctor
        public Driver(TestRail test)
        {
            _testRun = test;
            _testRun.StartTestRail();
            foreach (KeyValuePair <string, List <string>> case_id_section in _testRun.GetIDCaseInSection())          
                _sectionCaseToRun.Add(case_id_section.Key, case_id_section.Value);
            foreach (KeyValuePair<string, string> test_case_name in _testRun.GetTestCaseName)
                _testCase.Add(test_case_name.Key, test_case_name.Value);
        }
        //methods
        public void NavigateDriver(IWebDriver driver)
        {          
            //foreach (string topSite in _testRun.TopOnclick)
            //  Console.WriteLine(topSite);

            //foreach (string c in CaseToRun)
            //    Console.WriteLine(c);
            //return;

            _publishers = new PublisherTarget();
            _driverSettings = _publishers.GetDriverSettings(_testCase);
            ParserPage parsePage = new ParserPage();
            URLActual urlSwap = new URLActual();
            foreach (PublisherTarget driverSet in _driverSettings)
            {
                if (parsePage.IsZoneOnTestCase(driverSet.ZoneIds))
                {
                    _isZoneOnTestCase = true;
                    try
                    {
                        urlSwap.TestUrlForSwap = driverSet.Url;
                        driverSet.Url = urlSwap.TestUrlForSwap;
                        driver.Navigate().GoToUrl(driverSet.Url);
/* подготовка сайта для теста, 
* процедура hard code для сайтов,
* где нужно выполнить определенный набор действий для появления тега*/
                        _changeTargetUrl(driver);
                        while ((driver.WindowHandles.Count) > 1)
                        {
                            _closeOtherWindows(driver);
                            driver.SwitchTo().Window(driver.WindowHandles.ElementAt(0));
                        }
                        if (driver.Title.Contains("недоступен") || driver.Title.Contains("недоступна") || driver.Title.Contains("Проблема при загрузке страницы"))
                        {
                            _isLoadPage = false;
                            _isLandChecked = false;
                            _endTest(driver, driverSet, _getCaseIDForTestStatus(driver));
                            continue;
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
                                    _endTest(driver, driverSet, _getCaseIDForTestStatus(driver));
                                    break;
                                }
                            }
                            else
                            {
                                _endTest(driver, driverSet, _getCaseIDForTestStatus(driver));
                                break;
                            }
                        } //end while
                        /* Проверка на открытие после того, как все показы уже были
                         * Подключить тогда, когда будут доступны настройки зоны из ADP по API
                                    try
                                    {
                                        driver.SwitchTo().Window(driver.WindowHandles.ElementAt(0)).SwitchTo().ActiveElement().Click();
                                    }
                                    catch { }
                        */
                        if ((driver.WindowHandles.Count) == 1 && driverSet.CountShowPopup == 0 && _isLandChecked)
                            _endTest(driver, driverSet, _getCaseIDForTestStatus(driver));

                        else if (_isLandChecked && _isOnClick)
                            _endTest(driver, driverSet, _getCaseIDForTestStatus(driver));
                    }
                    catch (Exception e) { Console.WriteLine(e); }
                }
                else
                {
                    _endTest(driver, driverSet, _getCaseIDForTestStatus(driver));
                }         
            }//end foreach           
            }//end of function
private void _changeTargetUrl(IWebDriver driver)
{
    try
    {
        switch (driver.Url)
        {
            case "http://www.clipconverter.cc/":
                driver.FindElement(By.XPath("//*[@id='mediaurl']")).Click();
                driver.FindElement(By.XPath("//*[@id='mediaurl']")).SendKeys("http://www.youtube.com/watch?v=IhhJqJV_u6M");
                driver.FindElement(By.XPath("//*[@id='submiturl']")).Click();
                Thread.Sleep(10000);
                driver.FindElement(By.XPath("//*[@id='submitconvert']/input")).Click();
                Thread.Sleep(10000);
                break;
            default: break;
        }
    }
    catch (WebDriverException e) { Console.WriteLine(e); }
}
private string _getCaseIDForTestStatus(IWebDriver driver)
{
    if (_sectionCaseToRun.Count > 1)
    {
        foreach (KeyValuePair<string, List<string>> case_id in _sectionCaseToRun)
            foreach (string id in case_id.Value)
            {
                if (driver.GetType().Name.Contains(case_id.Key))
                {
                    /*Возвращает case_id для testrail
                     * а после этого id удаляется из списка, 
                     * да в такой последовательности работает корректно*/                     
                    case_id.Value.Remove(id);
                    return id;
                }
            }
        return "case_id is not found";
    }
    else
    {
        foreach (KeyValuePair<string, List<string>> case_id in _sectionCaseToRun)
            foreach (string id in case_id.Value)
            {             
                case_id.Value.Remove(id);
                return id;
            }
    }
    return null;
} 
private void _closeOtherWindows(IWebDriver driver)
{
        try
        {
            driver.SwitchTo().Window(driver.WindowHandles.ElementAt(1)).Close();
            if ((driver.WindowHandles.Count) > 1)
                 _acceptAlert(driver);
        }
        catch { }
}
private void _onclickProgress(IWebDriver driver, PublisherTarget d_setting)
{
    try
    {
        if ((driver.WindowHandles.Count) > 1)
        {
            _isOnClick = true;
            driver.SwitchTo().Window(driver.WindowHandles.ElementAt(1)).Close();
            Thread.Sleep(2000);
            if ((driver.WindowHandles.Count) > 1)
            {
                _acceptAlert(driver);
                if ((driver.WindowHandles.Count) > 1)
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
private void _endTest(IWebDriver driver, PublisherTarget driverSet, string caseID)
{
    string successMessage = null, errorMessage = null, commentMessage = null, retestMessage = null;
    if (!_isLandChecked)
    {
        if (_isLoadPage)
        {
            errorMessage = driver.Url + " Landing is " + _isLandChecked + "\nТег на странице не найден";
            commentMessage = "Landing is " + _isLandChecked + "\nТег на странице не найден";
            Console.Error.WriteLine(driver.Url + errorMessage);
            _testRun.SetStatus(caseID, _testRun.Status = Status.Failed, errorMessage, commentMessage);
        }
        else
        {
            if (!_isZoneOnTestCase)
                commentMessage = " Для данного кейса нет ZoneID";
            else
                commentMessage = " Веб-страница недоступна";
            errorMessage = driver.Url + commentMessage;
            Console.Error.WriteLine(errorMessage);
            _testRun.SetStatus(caseID, _testRun.Status = Status.Blocked, errorMessage, commentMessage);
        }
    }
    
    if (!_isOnClick && _isLandChecked)
    {
        errorMessage = driver.Url + " Во время клика не отработал показ. На сайте присутствует наш тег";
        commentMessage = "OnClick не отработал. Тег есть на странице";
        Console.Error.WriteLine(driver.Url + " OnClick is " + _isOnClick);
        _testRun.SetStatus(caseID, _testRun.Status = Status.Failed, errorMessage, commentMessage);
    }

    //if ((driver.WindowHandles.Count) == 1 && driverSet.CountShowPopup == 0)
    if (_isOnClick && driverSet.CountShowPopup == 0)
    {
        successMessage = driver.Url + "\nLanding is - " + _isLandChecked;
        Console.WriteLine(successMessage + " " + _isLandChecked + " " + _isOnClick);
        _testRun.SetStatus(caseID, _testRun.Status = Status.Passed, successMessage, null);
    }
    else if (_isLandChecked && _isOnClick)
    {
        retestMessage = "Landing is " + _isLandChecked + " "
            + driver.Url + " OnClick: popups is " + driverSet.CountShowPopup +
            " & count of windows " + driver.WindowHandles.Count + "\nIn the testing process is NOT open our Landing" +
            "\nPlease, repeat this test";        
        Console.Error.WriteLine(errorMessage + " " + _isOnClick);
        _testRun.SetStatus(caseID, _testRun.Status = Status.Retest, retestMessage, null);
    }
}
    }
}
    

