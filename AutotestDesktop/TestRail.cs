using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    class TestRail
    {
        private APIClient client = new APIClient("https://propeller.testrail.net");
        private const string _login = "stepanov.guap@gmail.com";
        private const string _password = "302bis";
        private List<string> _createCases = new List<string>();
        private string _runID = "";
        private int _numberCase;
        private Dictionary<string, List<string>> _testRun;
        public Status status;
       public enum Status
        {
            Untestead,
            Passed,
            Blocked,
            Critical,
            Implementated,
            Failed
        };
       public TestRail()
        {
            client.User = _login;
            client.Password = _password;
            int SuiteID = 47;//int.Parse(Console.ReadLine());
            _testRun = new Dictionary<string, List<string>>();
            JArray Sections = (JArray)client.SendGet("get_sections/3&suite_id=" + SuiteID);
            //foreach (var s in Sections)
            //{
            //    Console.WriteLine(s.ToString());
            //}
            JArray Runs = (JArray)client.SendGet("get_runs/3&suite_id=" + SuiteID);

            foreach (var run in Runs)
            {
                if(Convert.ToBoolean(run["is_completed"]) == false)
                    _runID = run["id"].ToString();
            }
            JArray TestCases = (JArray)client.SendGet("get_tests/" + _runID);
            _numberCase = TestCases.Count / Sections.Count;
            foreach (var section in Sections)
            {
                _testRun.Add(section["name"].ToString(), new List<string>());
                //Console.WriteLine(TestCases.Count);
                //return;
                foreach (var testCase in TestCases.Take(_numberCase))
                {
                    //Console.WriteLine(test["id"] + " " + test["title"]);
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

        public void CreateRun(int suiteId, string nameSuite)
        {
            client.User = _login;
            client.Password = _password;

            JArray caseData = (JArray)client.SendGet("get_cases/3/&suite_id=" + suiteId);
            foreach (var c in caseData)
                    _createCases.Add(c["id"].ToString());
            var runData = new Dictionary<string, object>
            {
                    {"suite_id", suiteId},
                    {"name", nameSuite},
                    {"include_all", true},
                    {"description", "Автоматическое тестирование Desktop OnClick для браузеров: Chrome, FireFox, Opera, IE, Safari"},
                    {"case_ids", _createCases.ToArray()},
            };
        
            JObject runCreate = (JObject)client.SendPost("add_run/3", runData);
            Console.WriteLine("Test run is create.");
         }
     

        public List <string> GetRunCase(IWebDriver driver)
        {
            List<string> TCases = new List<string>();
              foreach (KeyValuePair<string, List<string>> testrun in _testRun)
                foreach (string value in testrun.Value)
                {
                  //  Console.WriteLine(driver.GetType().Name);
                    if (driver.GetType().Name.Contains(testrun.Key)) 
                        TCases.Add(value);
                }
              return TCases;
        }
        public void SetStatus(string caseID, int statusID, string resultMessage, string commentMessage)
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
    }

   
}
