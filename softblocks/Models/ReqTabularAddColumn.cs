using softblocks.data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace softblocks.Models
{
    public class ReqTabularAddColumn
    {
        public string AppId { get; set; }
        public string DataViewId { get; set; }
        public string FieldId { get; set; }
        public int Order { get; set; }
    }
}