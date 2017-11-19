using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace softblocks.Models
{
    public class ReqCreateArticle
    {
        public string Title { get; set; }
        public string Article { get; set; }
        public string Status { get; set; }
    }
}