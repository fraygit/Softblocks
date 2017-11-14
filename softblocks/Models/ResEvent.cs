using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace softblocks.Models
{
    public class ResEvent
    {
        public string id { get; set; }
        public string title { get; set; }
        public DateTime start { get; set; }
        public DateTime end { get; set; }
        public string color { get; set; }
    }
}