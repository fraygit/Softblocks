using softblocks.data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace softblocks.Models
{
    public class ReqAddPanel
    {
        public string AppModuleId { get; set; }
        public string PageId { get; set; }
        public PagePanel Panel { get; set; }
        public string ForeignId { get; set; }
    }
}