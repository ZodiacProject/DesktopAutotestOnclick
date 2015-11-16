using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Gurock.TestRail;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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

namespace AutotestDesktop
{
    public class TestRail
    {
        private APIClient client = new APIClient("https://propeller.testrail.net");
        private const string _login = "stepanov.guap@gmail.com";
        private const string _password = "302bis";
        private const string _onclick = "_onclick";
        private const string _monday = "44", _wednesday = "47";
        private const string _planMonday = "2225", _planWednesday = "2253";
        private string _runID = null;
        private string _suiteId = null;
        private string _planID = null;
        private JObject _topSites;
        private JObject _topZoneSites;
        private JArray _caseData;
        private JArray _runs;
        private JObject _plans;
        private List<string> _testRun;
        private Dictionary<string, string> _testCaseName;
        private Dictionary<string, List<string>> _sites;
        private List<string> _lRunID;
        private List<string> _lConfigNameTestRun;
        private List<string> _createCases = new List<string>();
        public Dictionary<string, string> GetTestCaseName { get { return _getRegularNameCase(); } }

        public string GetSuiteID
        {
            get { return _suiteId; }
            set
            {
                switch (value)
                {
                    case "Monday": 
                        _suiteId = _monday; 
                        _planID = _planMonday;
                        break;              
                    case "Wednesday": 
                        _suiteId = _wednesday;
                        _planID = _planWednesday;
                        break;
                    default:                    
                        break;
                }
            }
        }
        public string GetConfigNameTestRun { get { return _lConfigNameTestRun.First(); }}
        public Status Status;

        public void TakeParamToStartTestRail()
        {
            client.User = _login;
            client.Password = _password;            
            _testRun = new List<string>();
            _testCaseName = new Dictionary<string, string>();
            JArray Sections = (JArray)client.SendGet("get_sections/3&suite_id=" + _suiteId);             
            Console.WriteLine("\nRun ID: " + _lRunID.First());
            JArray TestCases = (JArray)client.SendGet("get_tests/" + _lRunID.First());          
            _lRunID.RemoveAt(_lRunID.Count - _lRunID.Count); // удаление первого run id в последовательности _lRunID
            _lConfigNameTestRun.RemoveAt(_lConfigNameTestRun.Count - _lConfigNameTestRun.Count); // удаление первого config name run в последовательности _lConfigNameTestRun
            foreach (var caseName in TestCases)
            {
                if (_testCaseName.ContainsKey(caseName["title"].ToString()))
                    continue;// если коллекция уже содержит такой ключ, то переход к следующей записи
                else
                    _testCaseName.Add(caseName["title"].ToString(), caseName["custom_zone_id"].ToString());
            }            
            foreach (var section in Sections.Take(TestCases.Count))
            {
                if (section["parent_id"].ToString() == "")
                    continue;
                else
               // _testRun.Add(section["name"].ToString(), new List<string>());           
                foreach (var testCase in TestCases.Take(_testCaseName.Count)) // кол-во уникальных тестов в section  
                {
                    // Console.WriteLine(testCase["id"] + " " + testCase["title"]);
                    try
                    {
                        _testRun.Add(testCase["id"].ToString());                        
                    }
                    catch { }
                }
                if (TestCases.Count > 0)
                for (int i = 0; i < _testCaseName.Count; i++)
                {
                    TestCases.RemoveAt(TestCases.Count - TestCases.Count);   
                }                    

            }            
        }
        public void GetCases(string sID)
        {
            client.User = _login;
            client.Password = _password;
            int i = 0;
            JArray caseData = (JArray)client.SendGet("get_cases/3/&suite_id=" + sID);
            foreach (var c in caseData)
                i++;
            Console.WriteLine(i);
        }
        private void CreateRun(string nameSuite)
        {
            client.User = _login;
            client.Password = _password;
            _caseData = (JArray)client.SendGet("get_cases/3/&suite_id=" + _suiteId);
            foreach (var c in _caseData)
                _createCases.Add(c["id"].ToString());
            var runData = new Dictionary<string, object>
            {
                    {"suite_id", _suiteId},
                    {"name", nameSuite},
                    {"include_all", true},
                    {"description", "Автоматическое тестирование Desktop OnClick для браузеров: Chrome, FireFox, Opera, Edge, Safari"},
                    {"case_ids", _createCases.ToArray()},
            };

            JObject runCreate = (JObject)client.SendPost("add_run/3", runData);
            Console.WriteLine("\nTest run is create.");
            Console.WriteLine("Test is running..." + DateTime.Now);
        }
        public void CreateSuite()
        {
            client.User = _login;
            client.Password = _password;

            var suiteData = new Dictionary<string, object>
            {
                {"name", "Clobal Test Onclick for top sites"},
                {"description", "Автоматическое тестирование OnClick по выгрузке из статистики за последние 3 месяца"},
            };
            JObject suiteCreate = (JObject)client.SendPost("add_suite/3", suiteData);
            Console.WriteLine("\nTest Suite is create.");
        }
        public void AddCases()
        {
            int count = 0;
            Console.WriteLine("Creation of test-cases in the suite...");
            client.User = _login;
            client.Password = _password;
            _getTopSitesOnClick();
            foreach (KeyValuePair<string, List<string>> site in _sites)
            {
                try
                {
                    var CaseData = new Dictionary<string, object>
                    {      
                    {"title", site.Key},
                    {"type_id", 6},
                    {"ptiority_id", 4},                   
                    {"custom_zone_id", String.Join("#", site.Value)},
                    };
                    JObject suiteCreate = (JObject)client.SendPost("add_case/10086", CaseData); //9446 for suite_id=117
                    if (count == 38)
                        Thread.Sleep(60000);
                    count++;
                }
                catch { }// (Exception e) { Console.WriteLine(e + "\n" + site.Key + " " + site.Value[count]); }
            }
            Console.WriteLine("\nTest case(s) is added. Count cases: " + count);
        }
        public void UpdateTestSuite(string suiteID, string newSuiteName)
        {
            client.User = _login;
            client.Password = _password;

            var CaseData = new Dictionary<string, object>
            {
                {"name", newSuiteName},
            };
            JObject suiteCreate = (JObject)client.SendPost("update_suite/" + suiteID, CaseData);
            Console.WriteLine("\nTest Suite is update.");
        }

        public List<string> GetIDCaseInSection()
        {
            List<string> TCases = new List<string>();
            foreach (string testrun in _testRun)
                //foreach (string value in testrun.Value)
                //{
                //  Console.WriteLine(driver.GetType().Name);
                /*Section's select Chrome, FireFox, Opera, Safari, IE 
                 *  Данный участок кода необходим для определения из какой секции нужно брать тест, в зависимости от того какой сейчас браузер тестируется 
                 * 
                 */
                //if (driver.GetType().Name.Contains(testrun.Key))                     
                TCases.Add(testrun);            
            return TCases;
        }
        private Dictionary<string, string> _getRegularNameCase()
        {
            Dictionary<string, string> TtestCase = new Dictionary<string, string>();
            foreach (KeyValuePair<string, string> testname in _testCaseName)
                TtestCase.Add(testname.Key, testname.Value);
            return TtestCase;
        }

        private void _getTopSitesOnClick()
        {
            _sites = new Dictionary<string, List<string>>();
            string ThisMonth = null;
            string ThreeLastMonth = null;
            DateTime thisDay = DateTime.Today;
            DateTime threeLastMonth = new DateTime(thisDay.Year, thisDay.Month - 3, thisDay.Day);

            ThisMonth = _getDateForJasonRequest(thisDay);
            ThreeLastMonth = _getDateForJasonRequest(threeLastMonth);

            _topSites = (JObject)client.GetTopSitesData(ThreeLastMonth + "&day_to=" + ThisMonth + "&platforms=1&dept=onclick&group=affiliate&cut[revenue]=more0&order=revenue+desc&limit=80");
            string url = null;
            foreach (var site in _topSites)
            {
                url = "http://" + site.Value.SelectToken("affiliate_name").ToString().Substring(0, site.Value.SelectToken("affiliate_name").ToString().Length - _onclick.Length); // какая жесть :)
                if (_enabledUrl(url))
                {
                    _topZoneSites = (JObject)client.GetTopSitesData(ThisMonth + "&day_to=" + ThisMonth + "&affiliates=" + site.Value.SelectToken("affiliate").ToString() + "&group=zone&cut[impressions]=more10&order=revenue&limit=30");
                    _sites.Add(url, new List<string>());

                    if (_topZoneSites.Count != 0)
                        foreach (var topZone in _topZoneSites)
                        {
                            if (Convert.ToInt64(topZone.Value.SelectToken("zone")) != 0)
                                _sites[url].Add(topZone.Value.SelectToken("zone").ToString());
                        }
                    else
                        _sites[url].Add("ZoneIsNull");
                }
            }
        }

        private bool _enabledUrl(string url)
        {
            if (!url.Contains("revshare")
                && !url.Contains("watchmovies")
                && !url.Contains("media")
                && !url.Contains("wizard")
                && !url.Contains("promoter")
                && !url.Contains("adf.ly")
                && !url.Contains("sergplugin")
                && !url.Contains("clipconverter")
                && !url.Contains("publited")
                && !url.Contains("bc.vc")
                && !url.Contains("uptobox")
                && !url.Contains("prolads")
                && !url.Contains("rapidgator")
                && !url.Contains("norm")
                && !url.Contains("testXML")
                && !url.Contains("movietube")
                && !url.Contains("tuto4pc")
                && !url.Contains("moevideo")
                && !url.Contains("vkpass")
                && !url.Contains("imgsrc")
                && !url.Contains("novamov")
                && !url.Contains("grandslam")
                && !url.Contains("videosites")
                && !url.Contains("ihost.md")
                && !url.Contains("videowood")
                && !url.Contains("vimplevideo")
                && !url.Contains("vivo"))
            { return true; }
            else
                return false;
        }
        private string _getDateForJasonRequest(DateTime date)
        {
            string month = null;
            if (date.Month < 10)
                month = date.Year + "-" + "0" + date.Month;
            else
                month = date.Year + "-" + date.Month;

            if (date.Day < 10)
                month += "-" + "0" + date.Day;
            else
                month += "-" + date.Day;
            return month;
        }

        public void SetStatus(string caseID, Status statusID, string resultMessage, string commentMessage)
        {
            if (!caseID.Equals("case_id is not found"))
            {
                client.User = _login;
                client.Password = _password;

                var addResultData = new Dictionary<string, object>
                     {

                    {"status_id", statusID}, 
                    {"comment", resultMessage},
                    {"custom_comment_test", commentMessage}
                     };
                JObject r = (JObject)client.SendPost("add_result/" + caseID, addResultData);
            }
            else
                Console.WriteLine(caseID);
        }
        public void CloseRun()
        {

            client.User = _login;
            client.Password = _password;
            var closeData = new Dictionary<string, object>
            {
                {"description", "Тест проведен и закрыт"}
            };
            try
            {

                JObject c = (JObject)client.SendPost("close_run/" + _runID, closeData);
                Console.WriteLine("Test run " + _runID + " successfully removed");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        public void DeleteRun(string runID)
        {
            _runID = runID;
            client.User = _login;
            client.Password = _password;
            var deleteData = new Dictionary<string, object>
            {
                {"description", "Тест проведен и закрыт"}
            };
            try
            {

                JObject c = (JObject)client.SendPost("delete_run/" + _runID, deleteData);
                Console.Clear();
                Console.WriteLine("Test run " + _runID + " successfully removed");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        public void GetRunsProject()
        {
            Console.WriteLine("Topical test-runs:");
            client.User = _login;
            client.Password = _password;
            _runs = (JArray)client.SendGet("get_runs/3");
            foreach (var run in _runs)
            {
                if (Convert.ToBoolean(run["is_completed"]) == false)
                    Console.WriteLine("ID:" + run["id"].ToString() + "\tName:" + run["name"]);
            }
            Console.WriteLine();
        }
        public int GetPlansProject(string nSuite)
        {
            Console.Write("Topical test-plans: ");
            client.User = _login;
            client.Password = _password;
            GetSuiteID = nSuite;
            _lRunID = new List<string>();
            _lConfigNameTestRun = new List<string>();
            _plans = (JObject)client.SendGet("get_plan/" + _planID);            
            foreach (var plan in _plans)
            {
                if (plan.Key == "id")
                    Console.WriteLine(plan.Value);
                if (plan.Key == "entries")
                    foreach (var entries in plan.Value)
                    {
                        foreach (var ent_runs in entries["runs"])
                        {
                            _lRunID.Add(ent_runs["id"].ToString());
                            _lConfigNameTestRun.Add(ent_runs["config"].ToString());
                            Console.WriteLine("Run ID: " + ent_runs["id"].ToString() + " " + ent_runs["config"].ToString());
                        }
                    }
                                                                                                                      
            }            
            Console.WriteLine();
            return _lRunID.Count;
        }
        public void isTheRunAlreadyExists(string nSuite)
        {
            client.User = _login;
            client.Password = _password;
            bool flag = false;
            GetSuiteID = nSuite; // get suite id for test-run
            _runs = (JArray)client.SendGet("get_runs/3");
            foreach (var run in _runs)
            {
                if (Convert.ToBoolean(run["is_completed"]) == false)
                    if (run["name"].ToString() == nSuite)
                    {
                        _runID = run["id"].ToString();
                        flag = true;
                    }
            }
            if (!flag)
            {
                CreateRun(nSuite);
            }
        }        
        public void GetSuitesOfProject()
        {
            Console.WriteLine("Test Suites now:");
            client.User = _login;
            client.Password = _password;
            JArray SuiteData = (JArray)client.SendGet("get_suites/3");
            Console.WriteLine("ID\tName");
            foreach (var suite in SuiteData)
                Console.WriteLine(" " + suite["id"] + "\t" + suite["name"]);
            Console.WriteLine();
        }
    }
    public enum Status
    {
        Untestead,
        Passed = 1,
        Blocked = 2,
        Retest = 4,
        Implementated,
        Failed = 5
    }
}
