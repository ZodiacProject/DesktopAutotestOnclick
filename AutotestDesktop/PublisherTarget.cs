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
        private Dictionary <string, string> _dataCase;
        public string Url { get; set; }
        public string ZoneIds { get; private set; }
        public string TargetClick { get; private set; }
        public int CountShowPopup { get; set; }
        public int Interval { get; private set; }
        public int StepCase { get; private set; }
        public List<PublisherTarget> DriverSetting;

        public List<PublisherTarget> GetDriverSettings(Dictionary<string, string> DataCase)
        {
            int numberStep = 0;
            _dataCase = DataCase;
            DriverSetting = new List<PublisherTarget>();
            foreach (KeyValuePair <string, string> dataCase in DataCase)
            {
                DriverSetting.Add(new PublisherTarget()
                {
                    Url = dataCase.Key,
                    ZoneIds = dataCase.Value, //_GetZoneID(nameCase_Url.Key)
                    CountShowPopup = _GetShowPopup(dataCase.Key),
                    Interval = _GetInterval(dataCase.Key),
                    TargetClick = _GetTargetClick(dataCase.Key),
                    StepCase = numberStep++
                });
            }
                return DriverSetting;
        }
        private string _GetZoneID (string urlForFindZoneID)
        {
            switch (urlForFindZoneID)
            {
                case "http://putlocker.is": return "10802";
                   
                case "http://thevideos.tv": return "90446";
  
                case "http://www13.zippyshare.com/v/94311818/file.html": return "180376";

                case "http://um-fabolous.blogspot.ru": return "199287";

                case "http://www.flashx.tv/&?": return "119133";
                default: return "NOTZONE";
            }         
        }
        private int _GetShowPopup (string urlForFindZoneID)
        {
            switch (urlForFindZoneID)
            {
                case "http://putlocker.is": return 1;

                case "http://thevideos.tv": return 3;

                case "http://www13.zippyshare.com/v/94311818/file.html": return 2;

                case "http://um-fabolous.blogspot.ru": return 3;

                case "http://www.flashx.tv/&?": return 1;
                default: return 1;
            }
        }
        private int _GetInterval (string urlForFindZoneID)
        {
            switch (urlForFindZoneID)
            {
                case "http://putlocker.is": return 10000;

                case "http://thevideos.tv": return 45000;

                case "http://www13.zippyshare.com/v/94311818/file.html": return 45000;

                case "http://um-fabolous.blogspot.ru": return 45000;

                case "http://www.flashx.tv/&?": return 20000;
                default: return 20000;
            }
        }
        private string _GetTargetClick (string urlForFindZoneID)
        {
           switch (urlForFindZoneID)
            {
                case "http://thevideos.tv": return "morevids";         
                default: return null;
            }
        }

        }
  }
