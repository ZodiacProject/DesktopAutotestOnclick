using System;
using Newtonsoft.Json.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Drawing.Imaging;
using Gallio.Framework;
using Gallio.Model;
using MbUnit.Framework;

namespace AutotestDesktop
{
    class Driver
    {        
        private IWebDriver _driver;      
        private List<PublisherTarget> _driverSettings; // for many publishers
        private PublisherTarget _publishers;
        private List<string> TopSitesOnClick = new List<string>();
        private List <string> _sectionCaseToRun = new List<string>();
        private Dictionary<string, string> _testCase = new Dictionary<string, string>();
        private Dictionary<string, List<string>> _browsers = new Dictionary<string, List<string>>() 
        { 
            {"chrome", new List<string>(){"39","46","beta"}},
            {"firefox", new List<string>(){"34","41","beta"}},
            {"internet explorer", new List<string>(){"11"}},            
            {"safari", new List<string>(){"9"}},
            {"opera", new List<string>(){"12"}}
        };    
        private TestRail _testRun;
        private ParserPage _parsePage;
        private URLActual _urlSwap;
        private bool _isLandChecked;
        private bool _isOnClick;
        private bool _isLoadPage;
        private bool _isZoneOnTestCase;
        private string _statusSauceLabs = null;
//Constuctor
        public Driver(TestRail test)
        {
            _testRun = test;                        
            _publishers = new PublisherTarget();            
            _parsePage = new ParserPage();
            _urlSwap = new URLActual();
        }
//SauceLabs
        public void SauceLabsTest()
        {
            string platform = _testRun.GetConfigNameTestRun; // получение config name test run (имя платформы)
            _testRun.TakeParamToStartTestRail(); // получение test-case for test run            
                _sectionCaseToRun.Clear();
                _testCase.Clear();                       
            foreach (string case_id_section in _testRun.GetIDCaseInSection())
                _sectionCaseToRun.Add(case_id_section);
            foreach (KeyValuePair<string, string> test_case_name in _testRun.GetTestCaseName)
                _testCase.Add(test_case_name.Key, test_case_name.Value);
                             
                foreach (KeyValuePair<string, List<string>> browser in _browsers)
                    {
                        if (browser.Key == "safari" && (platform == "Windows XP" || platform == "Windows 7" || platform == "Windows 8.1" || platform == "Windows 10" || platform == "Linux"))
                            continue;
                        if (browser.Key == "opera" && (platform == "Windows 8.1" || platform == "Windows 10" || platform == "OS X 10.11"))
                            continue;                        
                        if (browser.Key == "internet explorer" && (platform == "Windows XP" || platform == "OS X 10.11" || platform == "Linux"))
                            continue;
                        if (browser.Key == "firefox" && platform == "OS X 10.11")
                            continue;
                        foreach (string browser_version in browser.Value)
                        {
                            _Setup(browser.Key, browser_version, platform);                   
                            _NavigateDriver(platform, browser.Key, browser_version);
                            _CleanUp();
                        }                                                                          
                    }                            
        }
//methods
        private void _NavigateDriver(string plfName, string brwName, string brwVersion)
        {            
            _driverSettings = _publishers.GetDriverSettings(_testCase);
            if (brwVersion == "beta")
                brwVersion += "." + brwName;
                                                 
            foreach (PublisherTarget driverSet in _driverSettings)
            {
                try
                {                                        
                    _isZoneOnTestCase = false;
                    _isLoadPage = false;
                    _isLandChecked = false;
                    _isOnClick = false;
                    if (_parsePage.IsZoneOnTestCase(driverSet.ZoneIds))
                    {
                        _isZoneOnTestCase = true;                  
                        _urlSwap.TestUrlForSwap = driverSet.Url;
                        driverSet.Url = _urlSwap.TestUrlForSwap;
                        _driver.Navigate().GoToUrl(driverSet.Url);
                        Console.WriteLine(plfName + ": " + brwName + " ver " + brwVersion + "\nUrl: " + driverSet.Url);
                        /* подготовка сайта для теста, 
                        * процедура hard code для сайтов,
                        * где нужно выполнить определенный набор действий для появления тега*/
                        if (_notLoadPage(_driver.Title))
                        {
                            _isLoadPage = false;
                            _endTest(_driver, driverSet, brwVersion);                         
                            continue;
                        }
                        else
                        {
                            _isLoadPage = true;
                            //_changeTestScripts(_driver);


                            if (_parsePage.FindZoneOnPage(_driver, driverSet.Url, driverSet.ZoneIds))
                                _isLandChecked = true;
                            else
                                _isLandChecked = false;
                            while (driverSet.CountShowPopup != 0)
                            {
                                _driver.SwitchTo().Window(_driver.WindowHandles.ElementAt(0)).SwitchTo().ActiveElement().Click();
                                Thread.Sleep(3000);
                                if (_isLandChecked)
                                {
                                    _onclickProgress(_driver, driverSet);
                                    if (!_isOnClick && _isLoadPage)
                                    {
                                        _endTest(_driver, driverSet, brwVersion);
                                        break;
                                    }
                                }
                                else
                                {
                                    _endTest(_driver, driverSet, brwVersion);
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
                            if ((_driver.WindowHandles.Count) == 1 && driverSet.CountShowPopup == 0 && _isLandChecked)
                            {
                                _endTest(_driver, driverSet, brwVersion);
                            }
                            else if (_isLandChecked && _isOnClick)
                            {
                                _endTest(_driver, driverSet, brwVersion);
                            }
                        } //end else _isloadPage = true;                                          
                    }
                    else
                    {
                        _endTest(_driver, driverSet, brwVersion);
                    }                    
                }// end try
                catch 
                {
                   // _getCaseIDForTestStatus(_driver, brwVersion);                 
                } // если происходт crash, то будет удален из списка case_id тот id тест, на котором покрашился тест
            }//end foreach            
            }//end of function
private IWebDriver _Setup(string browser, string version, string platform)
{
    // construct the url to sauce labs
    Uri commandExecutorUri = new Uri("http://ondemand.saucelabs.com/wd/hub");
    // set up the desired capabilities    
    DesiredCapabilities desiredCapabilites = new DesiredCapabilities(browser, version, Platform.CurrentPlatform); // set the desired browser
    desiredCapabilites.SetCapability("platform", platform); // operating system to use
    desiredCapabilites.SetCapability("username", Constants.SAUCE_LABS_ACCOUNT_NAME); // supply sauce labs username
    desiredCapabilites.SetCapability("accessKey", Constants.SAUCE_LABS_ACCOUNT_KEY);  // supply sauce labs account key
    desiredCapabilites.SetCapability("name", TestContext.CurrentContext.Test.Name = DateTime.Today.DayOfWeek.ToString() + " Onclick test"); // give the test a name

    try
    {
        // start a new remote web driver session on sauce labs
        _driver = new FirefoxDriver(desiredCapabilites);//new RemoteWebDriver(commandExecutorUri, desiredCapabilites);
        _driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(30));
    }
    catch (WebDriverException e) { Console.WriteLine(e); }
    return _driver;
}
private void _CleanUp()
{
    Console.WriteLine("# Saucelabs is " + _statusSauceLabs);
    try
    {
        // get the status of the current test
        switch (_statusSauceLabs)
        {
            case "passed": ((IJavaScriptExecutor)_driver).ExecuteScript("sauce:job-result=" + _statusSauceLabs); //isStatus = TestContext.CurrentContext.Outcome.Status == TestStatus.Passed;
                break;
            case "failed": ((IJavaScriptExecutor)_driver).ExecuteScript("sauce:job-result=" + _statusSauceLabs);
                break;
            case "blocked": ((IJavaScriptExecutor)_driver).ExecuteScript("sauce:job-result=skipped");
                break;
            case "retest": ((IJavaScriptExecutor)_driver).ExecuteScript("sauce:job-result=inconclusive");
                break;
            default: ((IJavaScriptExecutor)_driver).ExecuteScript("sauce:job-result=inconclusive");
                break;
        }
    }
    finally
    {
        _driver.Quit();
    }
}
private void _changeTestScripts(IWebDriver driver)
{    
    try
    {
        switch (driver.Url)
        {            
            case "http://www.dardarkom.com/28365-watch-and-download-drillbit-taylor-2008-online.html": driver.FindElement(By.XPath("//*[@id='txtselect_marker']")).Click();               
                break;
            case "http://dreamfilmhd.org/movies/details/683244446-dead-rising-watchtower/": driver.FindElement(By.XPath("//*[@id='viplayer_display']")).Click();               
                break;
            case "http://www.cloudy.ec/v/cf68e58f56d11": driver.FindElement(By.XPath("//*[@id='player']")).Click();
                break;
            case "http://www.filmesonlinegratis.net/assistir-escola-de-espioes-dublado-online.html": driver.FindElement(By.XPath("/html/body/script[3]")).Click();           
                break;
            case "http://megafilmeshd.net/henrique-iv-o-grande-rei-da-franca/": driver.FindElement(By.XPath("//*[@id='left']/div[1]/input")).Click();               
                break;
            default: break;
        }
    }
    catch { }
}
private bool _notLoadPage(string p)
        {
            if (p.Contains("недоступен")
               || p.Contains("недоступна")
               || p.Contains("Проблема при загрузке страницы")
               || p.Contains("error")
               || p.Contains("Forbidden")
               || p.Contains("Web server is down"))            
            { return true; }
            else
                return false;
        }
private string _getCaseIDForTestStatus(IWebDriver driver, string brwVersion)
{   
        foreach (string case_id in _sectionCaseToRun)
        {            
            /*Возвращает case_id для testrail
             * а после этого id удаляется из списка, 
             * да в такой последовательности работает корректно*/
            _sectionCaseToRun.Remove(case_id);
            return case_id;
        }
        return "case_id is not found";
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
            driver.SwitchTo().Window(driver.WindowHandles.ElementAt(1));
//wait load page            
            driver.Close();
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
private void _endTest(IWebDriver driver, PublisherTarget driverSet, string brVer)
{
    try
    {
        if (!driver.Url.Contains(driverSet.Url))
            Console.WriteLine("ERROR! " + driver.Url + " " + driverSet.Url);
        string successMessage = null, errorMessage = null, commentMessage = null, retestMessage = null;
        _statusSauceLabs = null;
        if (!_isLandChecked)
        {
            if (_isLoadPage)
            {
                errorMessage = driver.Url + " Landing is " + _isLandChecked + "\nТег на странице не найден";
                commentMessage = "Landing is " + _isLandChecked + "\nТег на странице не найден";                
                _testRun.SetStatus(_getCaseIDForTestStatus(driver, brVer), _testRun.Status = Status.Failed, errorMessage, commentMessage);
                _statusSauceLabs = "failed";             
            }
            else
            {
                if (!_isZoneOnTestCase)
                    commentMessage = " Для данного кейса нет ZoneID";
                else
                    commentMessage = " Веб-страница недоступна";
                errorMessage = driverSet.Url + commentMessage;
                Console.Error.WriteLine(errorMessage);
                _testRun.SetStatus(_getCaseIDForTestStatus(driver, brVer), _testRun.Status = Status.Blocked, errorMessage, commentMessage);
                _statusSauceLabs = "blocked";
            }
        }

        if (!_isOnClick && _isLandChecked)
        {
            errorMessage = driver.Url + " Во время клика не отработал показ. На сайте присутствует наш тег";
            commentMessage = "OnClick не отработал. Тег есть на странице";
            _testRun.SetStatus(_getCaseIDForTestStatus(driver, brVer), _testRun.Status = Status.Failed, errorMessage, commentMessage);
            _statusSauceLabs = "failed";         
        }
        
        if (_isOnClick && driverSet.CountShowPopup == 0)
        {
            successMessage = driver.Url + "\nLanding is - " + _isLandChecked;
            _testRun.SetStatus(_getCaseIDForTestStatus(driver, brVer), _testRun.Status = Status.Passed, successMessage, null);
            _statusSauceLabs = "passed";           
        }
        else if (_isLandChecked && _isOnClick)
        {
            retestMessage = "Landing is " + _isLandChecked + " "
                + driver.Url + " OnClick: popups is " + driverSet.CountShowPopup +
                " & count of windows " + driver.WindowHandles.Count + "\nIn the testing process is NOT open our Landing" +
                "\nPlease, repeat this test";
            Console.Error.WriteLine(retestMessage + " " + _isOnClick);
            _testRun.SetStatus(_getCaseIDForTestStatus(driver, brVer), _testRun.Status = Status.Retest, retestMessage, null);
            _statusSauceLabs = "retest";
        }
    }
    catch { }
    Console.WriteLine();
}
    }
}
    

