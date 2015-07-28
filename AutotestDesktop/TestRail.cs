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
        private string _runID = null;
        private string _suiteId = "";
        private int _numberCase;
        private JObject _topSites;
        private JObject _topZoneSites; 
        private JArray _caseData;
        private Dictionary<string, List<string>> _testRun;
        private Dictionary<string, string> _testCaseName;
        private Dictionary<string, List<string>> _sites;
        private List<string> _createCases = new List<string>();
        public Dictionary<string, string> TestCaseName { get { return _getRegularNameCase(); } }

        public string GetSuiteID { get { return _suiteId; } set { _suiteId = value; } }
        public string RunID { set { _runID = value; } }   
        public Status Status;
     
       public void StartTestRail()
        {
            client.User = _login;
            client.Password = _password;
            int created_on = 0; // переменной присваивается самый полесденй по номеру test-run
            _testRun = new Dictionary<string, List<string>>();
            _testCaseName = new Dictionary<string, string>();       
            JArray Sections = (JArray)client.SendGet("get_sections/3&suite_id=" + _suiteId);
            JArray Runs = (JArray)client.SendGet("get_runs/3&suite_id=" + _suiteId);
            //foreach (var s in Sections)
            //{
            //    Console.WriteLine(s.ToString());
            //}
           
            if (String.IsNullOrEmpty(_runID)) // Создание нового test-run (когда run ID имеет пустое значение)
            {
                foreach (var run in Runs)
                {
                    if (Convert.ToBoolean(run["is_completed"]) == false)
                        if (Convert.ToInt32(run["created_on"]) > created_on)
                        {
                            created_on = Convert.ToInt32(run["created_on"]);
                            _runID = run["id"].ToString();
                        }
                }
            }
             JArray TestCases = (JArray)client.SendGet("get_tests/" + _runID);

             foreach (var caseName in TestCases.Take(TestCases.Count/Sections.Count))
             {
                 _testCaseName.Add(caseName["title"].ToString(), caseName["custom_zone_id"].ToString());                
             }

            _numberCase = TestCases.Count / Sections.Count;
            foreach (var section in Sections)
            {
                _testRun.Add(section["name"].ToString(), new List<string>());
                //Console.WriteLine(TestCases.Count);
                //return;
                foreach (var testCase in TestCases.Take(_numberCase))
                {
                   // Console.WriteLine(testCase["id"] + " " + testCase["title"]);
                    try
                    {
                        _testRun[section["name"].ToString()].Add(testCase["id"].ToString());
                        // (TestRun["name"].ToString()).Add(test["id"].ToString());
                        //      Console.WriteLine(section["name"] + " " + test["title"]);
                    }
                    catch (ArgumentException) { }
                }
                for (int i = _numberCase-1; i >= 0; i--)
                    TestCases.RemoveAt(i);

            }
            //  foreach (KeyValuePair<string, List<string>> testrun in TestRun)
            //    foreach (string value in testrun.Value)
            //Console.WriteLine("name = {0}, id = {1}", testrun.Key, value);
        }
       public void GetCases()
       {
           client.User = _login;
           client.Password = _password;
           int i = 0;
           JArray caseData = (JArray)client.SendGet("get_cases/3/&suite_id=117");
           foreach (var c in caseData)
                        i++;
           Console.WriteLine(i);
       }
        public void CreateRun(string suiteId, string nameSuite)
        {
            client.User = _login;
            client.Password = _password;
         //   _suiteId = suiteId;
            _caseData = (JArray)client.SendGet("get_cases/3/&suite_id=" + _suiteId);
            foreach (var c in _caseData)
                    _createCases.Add(c["id"].ToString());
            var runData = new Dictionary<string, object>
            {
                    {"suite_id", _suiteId},
                    {"name", nameSuite},
                    {"include_all", true},
                    {"description", "Автоматическое тестирование Desktop OnClick для браузеров: Chrome, FireFox, Opera, IE, Safari"},
                    {"case_ids", _createCases.ToArray()},
            };
        
            JObject runCreate = (JObject)client.SendPost("add_run/3", runData);
            Console.WriteLine("\nTest run is create.");
            Console.WriteLine("Test is running...");
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
                    JObject suiteCreate = (JObject)client.SendPost("add_case/9446", CaseData);
                    if (count == 38)
                        Thread.Sleep(60000);
                    count++;
                }
                catch (Exception e) { Console.WriteLine(e + "\n" + site.Key + " " + site.Value[count]); }
            }
            Console.WriteLine("\nTest case(s) is added.");
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
     
        public List <string> GetRunCase(IWebDriver driver)
        {
            List<string> TCases = new List<string>();
              foreach (KeyValuePair<string, List<string>> testrun in _testRun)
                foreach (string value in testrun.Value)
                {
                  //  Console.WriteLine(driver.GetType().Name);

//Section's select Chrome, FireFox, Opera, Safari, IE if (driver.GetType().Name.Contains(testrun.Key))                     
                        TCases.Add(value);
                }
              return TCases;
        }
       private Dictionary <string, string> _getRegularNameCase()
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

           _topSites = (JObject)client.GetTopSitesData(ThreeLastMonth + "&day_to=" + ThisMonth + "&dept=onclick&group=affiliate&cut[revenue]=more0&order=revenue+desc&limit=50");
           string url = null;         
           foreach (var site in _topSites)
           {
               url = "http://" + site.Value.SelectToken("affiliate_name").ToString().Substring(0, site.Value.SelectToken("affiliate_name").ToString().Length - _onclick.Length); // какая жесть :)
               _topZoneSites = (JObject)client.GetTopSitesData(ThisMonth + "&day_to=" + ThisMonth + "&affiliates=" + site.Value.SelectToken("affiliate").ToString() + "&group=zone&cut[impressions]=more100&order=revenue&limit=30");
               _sites.Add(url, new List <string>());

               if (_topZoneSites.Count != 0)
                   foreach (var topZone in _topZoneSites)
                   {
                       if (Convert.ToInt64(topZone.Value.SelectToken("zone")) != 0)
                           _sites[url].Add(topZone.Value.SelectToken("zone").ToString());
                   }
               else
                   _sites[url].Add("Zone Not Found");
           }
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
        public void CloseRun()
        {
           
            client.User = _login;
            client.Password = _password;
            var closeData =  new Dictionary <string, object>
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
            Console.WriteLine("Актуальные test-runs:");
            client.User = _login;
            client.Password = _password;
            JArray Runs = (JArray)client.SendGet("get_runs/3");
            foreach (var run in Runs)
            {
                if (Convert.ToBoolean(run["is_completed"]) == false)
                    Console.WriteLine("ID:" + run["id"].ToString() + "\tName:" + run["name"]);
            }
            Console.WriteLine();
        }
        public void GetSuitesOfProject()
        {
            Console.WriteLine("Test Suites now:");
            client.User = _login;
            client.Password = _password;
            JArray SuiteData = (JArray)client.SendGet("get_suites/3");
            Console.WriteLine("ID\tName");
            foreach (var suite in SuiteData)
                Console.WriteLine( " " + suite["id"] + "\t" + suite["name"]);
            Console.WriteLine();
        }
    }
   public enum Status
   {
       Untestead,
       Passed = 1,
       Blocked,
       Retest = 4,
       Implementated,
       Failed = 5
   }

   
}
