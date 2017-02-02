﻿using Chirp2017.Models;
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
            VerifyAPIKeys();
            TwitterService service = SetUpSearchService();

            //service.TraceEnabled = true;

            SearchOptions options = new SearchOptions();
            options.Lang = "en";
            options.Count = data.searchData.myNumber > 0 ? data.searchData.myNumber : 10;

            //author
            var searchString = "from:" + (String.IsNullOrWhiteSpace(data.searchData.myUserName) ? "" : data.searchData.myUserName);

            //subject
            if (!String.IsNullOrWhiteSpace(data.searchData.myKeyword))
            {
                searchString += " " + data.searchData.myKeyword;
            }

            //location “37.781157,-122.398720”
            if (!String.IsNullOrEmpty(data.searchData.myLocation))
            {
                var split = data.searchData.myLocation.Split(',');
                double lat = 0;
                double lng = 0;
                if (split.Length == 2 && double.TryParse(split[0], out lat) && double.TryParse(split[1], out lng))
                {
                    int radius = 5;
                    options.Geocode = new TwitterGeoLocationSearch(lat, lng, radius, TwitterGeoLocationSearch.RadiusType.Km);
                }
            }

            options.Q = searchString;
            options.IncludeEntities = true;
            TwitterSearchResult tweets;
            try
            {
                tweets = service.Search(options);
            }
            catch (OverflowException e)
            {
                //looks like tweetsharp has an overflow issue http://stackoverflow.com/q/19669609
                //play it cool- proper fix would be to branch source and make fix described here= http://stackoverflow.com/a/25018796
                return View(new SearchPageModel() { searchData = data.searchData });
            }
            if (tweets == null || tweets.Statuses.Count() == 0)
            {
                //no results broh, check if username is invalid if we can
                TwitterUser tweeter = new TwitterUser();//if no username was provided don't mark as invalid
                bool userProvided = !String.IsNullOrEmpty(data.searchData.myUserName);
                if (userProvided)
                {
                    //call here to avoid slowing down valid requests
                    tweeter = service.GetUserProfileFor(new GetUserProfileForOptions() { ScreenName = data.searchData.myUserName });
                }

                return View(new SearchPageModel() { searchData = data.searchData, usernameValid = !userProvided && tweeter != null });
            }

            var simplerResults = new List<TweetInfo>();
            foreach (var t in tweets.Statuses)
            {
                simplerResults.Add(new Models.TweetInfo
                {
                    Author = t.Author.ScreenName
                    ,timeStamp = t.CreatedDate.ToLocalTime() //display time in server time, not UTC
                    ,TweetString = t.Text
                });
            }

            return View(new SearchPageModel() { searchData = data.searchData, tweets = simplerResults });
        }

        private static TwitterService SetUpSearchService()
        {
            var appSettings = ConfigurationManager.AppSettings;
            var consumerKey = appSettings["TwitterAppKey"];
            var consumerSecret = appSettings["TwitterAppSecret"];
            var personalToken = appSettings["TwitterPersonalToken"];
            var personalSecret = appSettings["TwitterPersonalSecret"];
            TwitterService service = new TwitterService(consumerKey, consumerSecret, personalToken, personalSecret);
            return service;
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Contact()
        {
            return View();
        }

        private void VerifyAPIKeys()
        {
            var appSettings = ConfigurationManager.AppSettings;

            if (String.IsNullOrEmpty(appSettings["TwitterAppKey"]) ||
                String.IsNullOrEmpty(appSettings["TwitterAppSecret"]) ||
                String.IsNullOrEmpty(appSettings["TwitterPersonalToken"]) ||
                String.IsNullOrEmpty(appSettings["TwitterPersonalSecret"]))
            {
                throw new Exception("please add API keys to app settings");
            }
        }
    }
}