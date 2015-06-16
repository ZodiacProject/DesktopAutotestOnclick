using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutotestDesktop
{
    class PublisherTarget
    {
        public string Url;
        public string ZoneId;
        public string TargetClick;
        public int CountShowPopup;
        public int Interval;
        public int StepCase;
        public List<PublisherTarget> DriverSetting;
        public List<PublisherTarget> GetDriverSettings()
        {
            DriverSetting = new List<PublisherTarget>()
                   {
                    new PublisherTarget() { Url = "http://putlocker.is", ZoneId = "10802", CountShowPopup = 3, Interval = 10000, StepCase = 0},
                    //new PublisherTarget() { Url = "http://thevideos.tv/", ZoneId = "90446", CountShowPopup = 3, Interval = 45000, TargetClick = "morevids", StepCase = 1},              
                    //new PublisherTarget() { Url = "http://www13.zippyshare.com/v/94311818/file.html/", ZoneId = "180376", CountShowPopup = 2, Interval = 45000, StepCase = 2},
                    //new PublisherTarget() { Url = "http://um-fabolous.blogspot.ru/", ZoneId = "199287", CountShowPopup = 3, Interval = 45000, StepCase = 3},                
                    //new PublisherTarget() { Url = "http://www.flashx.tv/&?", ZoneId = "119133", CountShowPopup = 1, Interval = 20000, StepCase = 4},              
                    };
            return DriverSetting;

        }
    }
}
