using softblocks.data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace softblocks.Models
{
    public class ReqAddDropdown
    {
        public string AppId { get; set; }
        public string Name { get; set; }
        public List<DropdownItem> Items { get; set; }
    }
}