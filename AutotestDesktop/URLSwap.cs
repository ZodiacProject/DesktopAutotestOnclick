using Newtonsoft.Json.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Opera;
using OpenQA.Selenium.Safari;
using OpenQA.Selenium.Support;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutotestDesktop
{
    class URLSwap
    {
        private string url;
        public string TestUrlForSwap
        {
            get
            {
                return SwapUrl();
            }
            set
            {
                this.url = value;
            }
        }
      private string SwapUrl ()
        {
            switch (url)
            {
                case "http://zippyshare.com": return "http://www13.zippyshare.com/v/94311818/file.html";
                default: return url;
            }
        }
    }

}
