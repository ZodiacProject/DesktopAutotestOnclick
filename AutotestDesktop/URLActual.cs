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
    class URLActual
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
                case "http://clipconverter.cc": return "http://www.clipconverter.cc/download/e1EeIm6p/185043830/";
                case "http://embed.novamov.com": return "http://www.novamov.com/video/408e5d3185650";
                case "http://estadiofutebol.com": return "http://www.estadiofutebol.com/gratis/sporttv";
                case "http://www.novamov.com": return "http://www.novamov.com/video/408e5d3185650";
                case "http://openload.io": return "https://openload.co/f/gwRpCqBo2XY/Pic.jpg";
                case "http://unblocked3.co": return "https://mp3raid.unblocked.pw/";
                case "http://megafilmeshd.net": return "http://megafilmeshd.net/henrique-iv-o-grande-rei-da-franca/";
                case "http://filmesonlinegratis.net": return "http://www.filmesonlinegratis.net/assistir-cop-car-legendado-online.html";
                case "http://dardarkom.com ": return "http://www.dardarkom.com/28365-watch-and-download-drillbit-taylor-2008-online.html";
                case "http://gidonlinekino.com": return "http://gidonline.club/2015/01/poteryannyj-raj/";
                case "http://realvid.net": return "http://realvid.net/6pv9gooplw6j";
                case "http://vidto.me": return "http://vidto.me/ewo6sgd07w6l.html";
                case "http://streamin.to": return "http://streamin.to/tar6jfe8875w";
                case "http://vkpass.com": return "http://dreamfilmhd.org/movies/details/683244446-dead-rising-watchtower/";
                case "http://cima4u.tv": return "http://cima4u.tv/online/?p=690";
                case "http://kinoman.tv": return "http://kinoman.tv/film/skazani-na-shawshank";
                case "http://cloudy.ec": return "http://www.cloudy.ec/v/cf68e58f56d11";               
                default: return url;
            }
        } 
    }

}
