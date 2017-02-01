using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Chirp2017.Models
{
    public class SearchPageModel
    {
        public SearchData searchData { get; set; }
        public List<Chirp2017.Models.TweetInfo> tweets {get;set;}
        public bool usernameValid { get; set; }// = true;
    }
}