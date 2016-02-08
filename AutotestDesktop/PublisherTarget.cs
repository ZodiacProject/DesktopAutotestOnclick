using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Opera;
using OpenQA.Selenium.Safari;
using OpenQA.Selenium.Support;

namespace AutotestDesktop
{
    public class PublisherTarget
    {   
        public string Url { get; set; }
        public string ZoneIds { get; set; }
        public string CaseID { get; set; }
        public string BrName { get; set; }
        public string BrVersion { get; set; }
        public int CountShowPopup { get; set; }
        public int Interval { get; set; }
        public int StepCase { get; set; }
    }
}
