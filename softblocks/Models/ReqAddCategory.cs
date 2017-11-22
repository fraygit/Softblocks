using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace softblocks.Models
{
    public class ReqAddCategory
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string DiscussionId { get; set; }
    }
}