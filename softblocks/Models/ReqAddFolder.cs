using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace softblocks.Models
{
    public class ReqAddFolder
    {
        public string Name { get; set; }
        public string Parent { get; set; }
        public string FolderType { get; set; }
    }
}