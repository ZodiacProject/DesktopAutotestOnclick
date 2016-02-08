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
        private Dictionary<string, Status> _LcaseWithStatus;
        private Dictionary<string, string> _LurlTestCase;
        private Dictionary<string, string> _LcaseCommentCase;
        private TestRail _testRun;
        private bool _isLandChecked;
        private bool _isOnClick;
        private bool _isLoadPage;
        private bool _isZoneOnTestCase;
        private string _statusSauceLabs = String.Empty;
        private string _urlCheck = String.Empty;        
//Constuctor
        public Driver(TestRail test)
        {
            _testRun = test;                        
            _publishers = new PublisherTarget();                                  

        }
        //SauceLabs
        public void SauceLabsTest()
        {
            string platform = _testRun.TheCurrentPlatform; // получение config name test run (имя платформы)         
            _testRun.TakeParamToStartTestRail(); // получение test-case for test run                                  
            _LcaseWithStatus = new Dictionary<string, Status>();
            _LurlTestCase = new Dictionary<string, string>();
            _LcaseCommentCase = new Dictionary<string, string>();
            _driverSettings = _testRun.LDriverSetting;  // получение параметров для теста (url, zoneid, caseid, br_name, br_version)
            _NavigateDriver(platform, _driverSettings);

            _testRun.SetStatusForRun(_LcaseWithStatus, _LurlTestCase, _LcaseCommentCase);            
        }
        //methods
        private void _NavigateDriver(string plfName, List<PublisherTarget> _drSettings)
        {
            string temp_brVersion = null;
            foreach (PublisherTarget driverSet in _drSettings)
            {
                if (driverSet.BrVersion.Contains("beta"))
                {
                    temp_brVersion = driverSet.BrVersion;
                    temp_brVersion = temp_brVersion.Remove(temp_brVersion.IndexOf('.'));
                    _Setup(driverSet.BrName, temp_brVersion, plfName, driverSet.Url);
                }
                else
                    _Setup(driverSet.BrName, driverSet.BrVersion, plfName, driverSet.Url);

                try
                {
                    _isZoneOnTestCase = false;
                    _isLoadPage = false;
                    _isLandChecked = false;
                    _isOnClick = false;

                    if ((_driver.WindowHandles.Count) > 1)
                        _closeOtherWindows(_driver);

                    if (ParserPage.IsZoneOnTestCase(driverSet.ZoneIds))
                    {
                        _isZoneOnTestCase = true;
                        URLActual.TestUrlForSwap = driverSet.Url;
                        driverSet.Url = URLActual.TestUrlForSwap;

                        if (!String.IsNullOrEmpty(_urlCheck = ParserPage.GetFoundUrlCheck))
                            if (_urlCheck.Contains(driverSet.Url))
                                driverSet.Url = _urlCheck;

                        _driver.Navigate().GoToUrl(driverSet.Url);
                        Console.WriteLine(plfName + ": " + driverSet.BrName + " ver " + driverSet.BrVersion + "\nUrl: " + driverSet.Url);
                        /* подготовка сайта для теста, 
                        * процедура hard code для сайтов,
                        * где нужно выполнить определенный набор действий для появления тега*/
                        if (_notLoadPage(_driver.Title))
                        {
                            _isLoadPage = false;
                            _endTest(_driver, driverSet);
                            break;
                        }
                        else
                        {
                            _isLoadPage = true;
                            //_changeTestScripts(_driver);


                            if (ParserPage.FindZoneOnPage(_driver, driverSet.Url, driverSet.ZoneIds))
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
                                        _endTest(_driver, driverSet);
                                        break;
                                    }
                                }
                                else
                                {
                                    _endTest(_driver, driverSet);
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
                                _endTest(_driver, driverSet);
                            }
                            else if (_isLandChecked && _isOnClick)
                            {
                                _endTest(_driver, driverSet);
                            }
                        } //end 
                    }
                    else
                    {
                        _endTest(_driver, driverSet);
                    }
                }// end try
                catch { }
                _CleanUp(); //close test 
            }// end foreach                                                                     
        }//end of function
        private IWebDriver _Setup(string browser, string version, string platform, string NameTestUrl)
        {
            // construct the url to sauce labs
            Uri commandExecutorUri = new Uri("http://ondemand.saucelabs.com/wd/hub");
            // set up the desired capabilities    
            DesiredCapabilities desiredCapabilites = new DesiredCapabilities(browser, version, Platform.CurrentPlatform); // set the desired browser
            desiredCapabilites.SetCapability("platform", platform); // operating system to use
            desiredCapabilites.SetCapability("username", Constants.SAUCE_LABS_ACCOUNT_NAME); // supply sauce labs username
            desiredCapabilites.SetCapability("accessKey", Constants.SAUCE_LABS_ACCOUNT_KEY);  // supply sauce labs account key
            desiredCapabilites.SetCapability("name", TestContext.CurrentContext.Test.Name = NameTestUrl + " OnClick test"); // give the test a name


    try
    {
        // start a new remote web driver session on sauce labs
        _driver = new RemoteWebDriver(commandExecutorUri, desiredCapabilites); //new FirefoxDriver(desiredCapabilites);
        _driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(30));
    }
    catch (WebDriverException e) { Console.WriteLine(e); }
    return _driver;
}
private void _CleanUp()
{
    Console.WriteLine("# Saucelabs is " + _statusSauceLabs + "\n");
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
        _statusSauceLabs = null;
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

private void _getCaseIDForTestStatus(Status status, string drCaseID, string drUrl, string drComment)
{   
    _LcaseWithStatus.Add(drCaseID, status);
    _LurlTestCase.Add(drCaseID, drUrl);
    _LcaseCommentCase.Add(drCaseID, drComment);
    Console.WriteLine("case id " + drCaseID);
    Console.WriteLine();     
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
        private void _endTest(IWebDriver driver, PublisherTarget driverSet)
        {
            try
            {
                if (!driver.Url.Contains(driverSet.Url))
                    Console.WriteLine("ERROR! " + driver.Url + " " + driverSet.Url);
                string successMessage = null, errorMessage = null, commentMessage = null, retestMessage = null;
                if (!_isLandChecked)
                {
                    if (_isLoadPage)
                    {
                        errorMessage += driver.Url + " Landing is " + _isLandChecked + "\nТег на странице не найден";
                        commentMessage = "Landing is " + _isLandChecked + "\nТег на странице не найден";
                        _getCaseIDForTestStatus(Status.Failed, driverSet.CaseID, driverSet.Url, commentMessage); 
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
                        _getCaseIDForTestStatus(Status.Blocked, driverSet.CaseID, driverSet.Url, commentMessage); 
                        _statusSauceLabs = "blocked";
                    }
                }

                if (!_isOnClick && _isLandChecked)
                {
                    errorMessage = driver.Url + " Во время клика не отработал показ. На сайте присутствует наш тег";
                    commentMessage = "OnClick не отработал. Тег есть на странице";
                    _getCaseIDForTestStatus(Status.Failed, driverSet.CaseID, driverSet.Url, commentMessage); 
                    _statusSauceLabs = "failed";
                }

                if (_isOnClick && driverSet.CountShowPopup == 0)
                {                    
                    successMessage += driver.Url + "\nLanding is - " + _isLandChecked;
                    _getCaseIDForTestStatus(Status.Passed, driverSet.CaseID, driverSet.Url, null); 
                    _statusSauceLabs = "passed";
                }
                else if (_isLandChecked && _isOnClick)
                {
                    retestMessage = "Landing is " + _isLandChecked + " "
                        + driver.Url + " OnClick: popups is " + driverSet.CountShowPopup +
                        " & count of windows " + driver.WindowHandles.Count + "\nIn the testing process is NOT open our Landing" +
                        "\nPlease, repeat this test";
                    Console.Error.WriteLine(retestMessage + " " + _isOnClick);
                    _getCaseIDForTestStatus(Status.Retest, driverSet.CaseID, driverSet.Url, retestMessage); 
                   // _testRun.SetStatus(_getCaseIDForTestStatus(), _testRun.Status = Status.Retest, retestMessage, null);
                    _statusSauceLabs = "retest";
                }
            }
            catch { }
          
        }
    }
}


