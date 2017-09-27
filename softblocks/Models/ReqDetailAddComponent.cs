using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace softblocks.Models
{
    public class ReqDetailAddComponent
    {
        public string AppId { get; set; }
        public string DataViewId { get; set; }
        public string FieldId { get; set; }
        public string ComponentType { get; set; }
        public int ColWidth { get; set; }
        public int Order { get; set; }
        public string Text { get; set; }
    }
}