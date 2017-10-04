using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace softblocks.Models
{
    public class ReqAddData
    {
        public string appId { get; set; }
        public string foreignId { get; set; }
        public string data { get; set; }
        public string RootDataId { get; set; }
    }
}