using Chirp2017.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TweetSharp;

namespace Chirp2017.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {   
            return View();
        }

        [HttpPost]
        public ActionResult Search(SearchPageModel data)
        {
            var appSettings = ConfigurationManager.AppSettings;

            if (String.IsNullOrEmpty(appSettings["TwitterAppKey"]) ||
                String.IsNullOrEmpty(appSettings["TwitterAppSecret"]) ||
                String.IsNullOrEmpty(appSettings["TwitterPersonalToken"]) ||
                String.IsNullOrEmpty(appSettings["TwitterPersonalSecret"] ))
            {
                throw new Exception("please add API keys to app settings");
            }
            var consumerKey = appSettings["TwitterAppKey"];
            var consumerSecret = appSettings["TwitterAppSecret"];
            TwitterService service = new TwitterService(consumerKey, consumerSecret);

            var personalToken = appSettings["TwitterPersonalToken"];
            var personalSecret = appSettings["TwitterPersonalSecret"];
            service.AuthenticateWith(personalToken, personalSecret);
            service.TraceEnabled = true;
            SearchOptions options = new SearchOptions();
            options.Lang = "en";
            options.Count = data.searchData.myNumber > 0 ? data.searchData.myNumber : 10;
            var searchString = "from:" + data.searchData.myUserName;
            if (!String.IsNullOrWhiteSpace(data.searchData.myKeyword))
            {
                searchString += " " + data.searchData.myKeyword;
            }
            //“37.781157,-122.398720,1mi”
            //var split = data.myLocation.Split(',');
            //double lat;
            //double lng;
            //int radius = 5;
            //if(split.Length == 3 && double.TryParse()
            //{
            //    options.Geocode = new TwitterGeoLocationSearch((double)split[0],(double)split[1],(int)split[3]);
            //}
            
            options.Q = searchString;
            options.Resulttype = TwitterSearchResultType.Popular;
            options.IncludeEntities = false;
            TwitterSearchResult tweets;
            try
            {
                tweets = service.Search(options);
            }
            catch(OverflowException e)
            {
                //looks like tweetsharp has an overflow issue http://stackoverflow.com/q/19669609
                //play it cool
                return View(new SearchPageModel() { searchData = data.searchData });
            }
            if (tweets == null)
            {
                //no results broh
                return View(new SearchPageModel() { searchData = data.searchData });
            }

            var simplerResults = new List<TweetInfo>();
            foreach(var t in tweets.Statuses)
            {
                simplerResults.Add(new Models.TweetInfo
                {
                    Author = t.Author.ScreenName
                    ,timeStamp = t.CreatedDate.ToLocalTime() //to display time in server time, not UTC
                    ,TweetString = t.Text
                });
            }

            return View(new SearchPageModel() { searchData = data.searchData, tweets = simplerResults });
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Contact()
        {
            return View();
        }
    }
}