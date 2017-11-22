using softblocks.data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace softblocks.Models
{
    public class ResComment
    {
        public string Name { get; set; }
        public DateTime ReplyDate { get; set; }
        public Comment Comment { get; set; }
    }
}