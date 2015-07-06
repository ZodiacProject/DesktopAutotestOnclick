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
    class PublisherTarget
    {
        private List <string> _nameCase;
        public string Url {get; private set;}
        public string ZoneId {get; private set;}
        public string TargetClick {get; private set;}
        public int CountShowPopup { get; set; }
        public int Interval {get; private set;}
        public int StepCase {get; private set;}
      
        public List<PublisherTarget> DriverSetting;

        public List<PublisherTarget> GetDriverSettings(List <string> NameCase)
        {
            int numberStep = 0;
            _nameCase = NameCase;
            DriverSetting = new List<PublisherTarget>();
            foreach (string nameCase_Url in NameCase)
            {
    
                DriverSetting.Add(new PublisherTarget() 
                { 
                  Url = nameCase_Url, 
                  ZoneId = _getZoneID(nameCase_Url), 
                  CountShowPopup = _getShowPopup(nameCase_Url), Interval = _getInterval(nameCase_Url),
                  TargetClick = _getTargetClick(nameCase_Url), StepCase = numberStep++});
                }
                return DriverSetting;
        }
        private string _getZoneID (string urlForFindZoneID)
        {
            switch (urlForFindZoneID)
            {
                case "http://putlocker.is": return "10802";
                   
                case "http://thevideos.tv/": return "90446";
  
                case "http://www13.zippyshare.com/v/94311818/file.html": return "180376";

                case "http://um-fabolous.blogspot.ru/": return "199287";

                case "http://www.flashx.tv/&?": return "119133";
                default: return null;
            }         
        }
        private int _getShowPopup (string urlForFindZoneID)
        {
            switch (urlForFindZoneID)
            {
                case "http://putlocker.is": return 3;

                case "http://thevideos.tv/": return 3;

                case "http://www13.zippyshare.com/v/94311818/file.html": return 2;

                case "http://um-fabolous.blogspot.ru/": return 3;

                case "http://www.flashx.tv/&?": return 1;
                default: return 0;
            }
        }
        private int _getInterval (string urlForFindZoneID)
        {
            switch (urlForFindZoneID)
            {
                case "http://putlocker.is": return 10000;

                case "http://thevideos.tv/": return 45000;

                case "http://www13.zippyshare.com/v/94311818/file.html": return 45000;

                case "http://um-fabolous.blogspot.ru/": return 45000;

                case "http://www.flashx.tv/&?": return 20000;
                default: return 0;
            }
        }
        private string _getTargetClick (string urlForFindZoneID)
        {
            switch (urlForFindZoneID)
            {
                case "http://thevideos.tv/": return "morevids";         
                default: return null;
            }
        }
        }
  }
