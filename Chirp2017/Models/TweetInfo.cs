using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Chirp2017.Models
{
    public class TweetInfo
    {
        public string Author { get; set; }
        public string TweetString { get; set; }
        public DateTime timeStamp { get; set; }
    }
}